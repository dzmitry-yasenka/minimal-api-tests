using System.Net.Mime;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using MinimalApiTests.Extending.LibraryApi;
using MinimalApiTests.Extending.LibraryApi.Auth;
using MinimalApiTests.Extending.LibraryApi.Data;
using MinimalApiTests.Extending.LibraryApi.Models;
using MinimalApiTests.Extending.LibraryApi.Services;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
   // ApplicationName = "Library.API"
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AnyOrigin", policyBuilder => policyBuilder.AllowAnyOrigin());
});

builder.Configuration.AddJsonFile(@"appsettings.Local.json", true, true);
Console.WriteLine(builder.Configuration.GetValue<string>("Section:SubSection"));


builder.Services.AddAuthentication(ApiKeySchemeConstants.SchemeName)
    .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(ApiKeySchemeConstants.SchemeName, _ => { });
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new SqliteConnectionFactory(builder.Configuration.GetValue<string>("Database:ConnectionString")));
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<IBookService, BookService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.UseCors();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();


app.MapPost("books", [Authorize(AuthenticationSchemes = ApiKeySchemeConstants.SchemeName)] 
    async (Book book, IBookService bookService, IValidator<Book> validator) =>
{
    var validationResult = await validator.ValidateAsync(book);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }
    
    var created = await bookService.CreateAsync(book);
    if (!created)
    {
        return Results.BadRequest(new List<ValidationFailure>
        {
            new ValidationFailure("Isbn", "A book with this ISBN-13 already exists")
        });
    }

    return Results.CreatedAtRoute("GetBook", new {isbn = book.Isbn}, book);
})
    .WithName("CreateBook")
    .Accepts<Book>(MediaTypeNames.Application.Json)
    .Produces<Book>(StatusCodes.Status201Created)
    .Produces<IEnumerable<ValidationFailure>>(StatusCodes.Status400BadRequest)
    .WithTags("Books");

app.MapGet("books", async (IBookService bookService) =>
{
    var books = await bookService.GetAllAsync();
    return Results.Ok(books);
})
    .WithName("GetBooks")
    .Produces<IEnumerable<Book>>()
    .WithTags("Books");

app.MapGet(@"books/{isbn}", async (string isbn, IBookService bookService) =>
{
    var book = await bookService.GetByIsbnAsync(isbn);
    return book is null ? Results.NotFound() : Results.Ok(book);
})
    .WithName("GetBook")
    .Produces<Book>()
    .Produces(StatusCodes.Status404NotFound)
    .WithTags("Books");

app.MapGet("books/search", async ([FromQuery] string searchTerm, IBookService bookService) =>
{
    var books = await bookService.SearchByTitleAsync(searchTerm);
    return Results.Ok(books);
})
    .WithName("SearchBooks")
    .Produces<IEnumerable<Book>>()
    .WithTags("Books");

app.MapPut("books/{isbn}", async (string isbn, Book book, IBookService bookService, IValidator<Book> validator) =>
{
    if (book.Isbn != isbn)
    {
        return Results.BadRequest(new List<ValidationFailure>
        {
            new ValidationFailure("Isbn", "Url ISBN doesn't match ISBN from the body of the request")
        });
    }
    
    var validationResult = await validator.ValidateAsync(book);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }
    
    var updated = await bookService.UpdateAsync(book);
    return updated ? Results.Ok(book) : Results.NotFound();
})
    .WithName("UpdateBook")
    .Accepts<Book>(MediaTypeNames.Application.Json)
    .Produces<Book>(StatusCodes.Status200OK)
    .Produces<IEnumerable<ValidationFailure>>(StatusCodes.Status400BadRequest)
    .WithTags("Books");

app.MapDelete("books/{isbn}", async (string isbn, IBookService bookService) =>
{
    var deleted = await bookService.DeleteAsync(isbn);
    return deleted ? Results.NoContent() : Results.NotFound();
})
    .WithName("DeleteBook")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithTags("Books");

app.MapGet("status", () =>
{
    return Results.Extensions.Html(@"<!DOCTYPE html>
<html>
<head>
<title>Status page of the Library API</title>
</head>

<body>
<h1>Status is ok</h1>
<h3>Server seems to be working normally</h3>
</body>

</html>");
})
    .ExcludeFromDescription()
    .RequireCors("AnyOriginPolicy");

// DB initialization here
var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

app.Run();