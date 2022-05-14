using Dapper;
using MinimalApiTests.LetsBuild.LibraryApi.Data;
using MinimalApiTests.LetsBuild.LibraryApi.Models;

namespace MinimalApiTests.LetsBuild.LibraryApi.Services;

public class BookService : IBookService
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public BookService(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Book book)
    {
        var existingBook = await GetByIsbnAsync(book.Isbn);
        if (existingBook is not null)
        {
            return false;
        }

        var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            @"INSERT INTO Books (Isbn, Title, Author, ShortDescription, PageCount, ReleaseDate) 
                                VALUES (@Isbn, @Title, @Author, @ShortDescription, @PageCount, @ReleaseDate)", book);

        return result > 0;
    }

    public Task<Book?> GetByIsbnAsync(string isbn)
    {
        return Task.FromResult<Book?>(null);
    }

    public Task<IEnumerable<Book>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Book book)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(string isbn)
    {
        throw new NotImplementedException();
    }
}