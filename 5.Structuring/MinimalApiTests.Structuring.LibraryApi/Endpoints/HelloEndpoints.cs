using MinimalApiTests.Structuring.LibraryApi.Endpoints.Internal;

namespace MinimalApiTests.Structuring.LibraryApi.Endpoints;

public class HelloEndpoints : IEndpoints
{
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("hello", () => "Hello World from HelloEndpoints!");
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        
    }
}