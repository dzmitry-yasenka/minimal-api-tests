using System.Data;

namespace MinimalApiTests.LetsBuild.LibraryApi.Data;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}