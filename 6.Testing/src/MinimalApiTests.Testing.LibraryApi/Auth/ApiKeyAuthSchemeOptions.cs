using Microsoft.AspNetCore.Authentication;

namespace MinimalApiTests.Testing.LibraryApi.Auth;

public class ApiKeyAuthSchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = "VerySecret";
}