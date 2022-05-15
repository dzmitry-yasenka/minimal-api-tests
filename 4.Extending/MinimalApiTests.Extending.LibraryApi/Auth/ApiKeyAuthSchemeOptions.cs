using Microsoft.AspNetCore.Authentication;

namespace MinimalApiTests.Extending.LibraryApi.Auth;

public class ApiKeyAuthSchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = "VerySecret";
}