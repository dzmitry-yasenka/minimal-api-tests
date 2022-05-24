using System.Net.Mime;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using MinimalApiTests.Filtering.LibraryApi.Endpoints.Internal;
using MinimalApiTests.Filtering.LibraryApi.Filters;
using MinimalApiTests.Filtering.LibraryApi.Models;
using MinimalApiTests.Filtering.LibraryApi.Services;
using MinimalApiTests.Filtering.LibraryApi.Validators;

namespace MinimalApiTests.Filtering.LibraryApi.Endpoints;

public class LibraryEndpoints : IEndpoints
{
    private const string BaseRoute = "books";
    private const string LibraryEndpointsTag = "Books";
    
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(BaseRoute, CreateBookAsync)
            .WithName("CreateBook")
            .AddFilter<ValidationFilter<Book>>()
            .Accepts<Book>(MediaTypeNames.Application.Json)
            .Produces<Book>(StatusCodes.Status201Created)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .WithTags(LibraryEndpointsTag);

        app.MapGet(BaseRoute, GetAllBooksAsync)
            .WithName("GetBooks")
            .Produces<IEnumerable<Book>>()
            .WithTags(LibraryEndpointsTag);

        app.MapGet($"{BaseRoute}/{{isbn}}", GetBookByIsbnAsync)
            .WithName("GetBook")
            .Produces<Book>()
            .Produces(StatusCodes.Status404NotFound)
            .WithTags(LibraryEndpointsTag);

        app.MapGet($"{BaseRoute}/search", SearchBooksAsync)
            .WithName("SearchBooks")
            .Produces<IEnumerable<Book>>()
            .WithTags(LibraryEndpointsTag);

        app.MapPut($"{BaseRoute}/{{isbn}}", UpdateBookAsync)
            .WithName("UpdateBook")
            .AddFilter<ValidationFilter<Book>>()
            .Accepts<Book>(MediaTypeNames.Application.Json)
            .Produces<Book>()
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .WithTags(LibraryEndpointsTag);

        app.MapDelete($"{BaseRoute}/{{isbn}}", DeleteBookAsync)
            .WithName("DeleteBook")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags(LibraryEndpointsTag);
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IBookService, BookService>();
    }

    private static async Task<IResult> CreateBookAsync(Book book, IBookService bookService)
    {
        var created = await bookService.CreateAsync(book);
        if (!created)
            return Results.BadRequest(new List<ValidationFailure>
            {
                new("Isbn", "A book with this ISBN-13 already exists")
            }.ToResponse());

        return Results.CreatedAtRoute("GetBook", new {isbn = book.Isbn}, book);
    }

    private static async Task<IResult> GetAllBooksAsync(IBookService bookService)
    {
        var books = await bookService.GetAllAsync();
        return Results.Ok(books);
    }

    private static async Task<IResult> GetBookByIsbnAsync(string isbn, IBookService bookService)
    {
        var book = await bookService.GetByIsbnAsync(isbn);
        return book is null ? Results.NotFound() : Results.Ok(book);
    }

    private static async Task<IResult> SearchBooksAsync([FromQuery] string searchTerm, IBookService bookService)
    {
        var books = await bookService.SearchByTitleAsync(searchTerm);
        return Results.Ok(books);
    }

    private static async Task<IResult> UpdateBookAsync(string isbn, Book book, IBookService bookService)
    {
        if (book.Isbn != isbn)
            return Results.BadRequest(new List<ValidationFailure>
            {
                new("Isbn", "Url ISBN doesn't match ISBN from the body of the request")
            }.ToResponse());

        var updated = await bookService.UpdateAsync(book);
        return updated ? Results.Ok(book) : Results.NotFound();
    }

    private static async Task<IResult> DeleteBookAsync(string isbn, IBookService bookService)
    {
        var deleted = await bookService.DeleteAsync(isbn);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}