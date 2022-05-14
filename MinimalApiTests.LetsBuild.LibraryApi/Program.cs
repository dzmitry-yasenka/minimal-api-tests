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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// DB initialization here
var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

app.MapPost("books", async (Book book, IBookService bookService) =>
{
    var created = await bookService.CreateAsync(book);
    if (!created)
    {
        return Results.BadRequest(new
        {
            errorMessage = "A book with this ISBN-13 is already exists"
        });
    }

    return Results.Created($"/books/{book.Isbn}", book);
});

app.Run();