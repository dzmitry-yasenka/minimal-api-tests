using MinimalApiTests.Testing.LibraryApi.Endpoints.Internal;

namespace MinimalApiTests.Testing.LibraryApi.Endpoints;

public class StatusEndpoints : IEndpoints
{
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
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
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }
}