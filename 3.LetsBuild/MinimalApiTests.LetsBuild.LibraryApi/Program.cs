using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using MinimalApiTests.LetsBuild.LibraryApi.Data;
using MinimalApiTests.LetsBuild.LibraryApi.Models;
using MinimalApiTests.LetsBuild.LibraryApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new SqliteConnectionFactory(builder.Configuration.GetValue<string>("Database:ConnectionString")));
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<IBookService, BookService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("books", async (Book book, IBookService bookService, IValidator<Book> validator) =>
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

    return Results.Created($"/books/{book.Isbn}", book);
});

app.MapGet("books", async (IBookService bookService) =>
{
    var books = await bookService.GetAllAsync();
    return Results.Ok(books);
});

app.MapGet(@"books/{isbn}", async (string isbn, IBookService bookService) =>
{
    var book = await bookService.GetByIsbnAsync(isbn);
    return book is null ? Results.NotFound() : Results.Ok(book);
});

app.MapGet("books/search", async ([FromQuery] string searchTerm, IBookService bookService) =>
{
    var books = await bookService.SearchByTitleAsync(searchTerm);
    return Results.Ok(books);
});

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
});

app.MapDelete("books/{isbn}", async (string isbn, IBookService bookService) =>
{
    var deleted = await bookService.DeleteAsync(isbn);
    return deleted ? Results.NoContent() : Results.NotFound();
});

// DB initialization here
var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

app.Run();