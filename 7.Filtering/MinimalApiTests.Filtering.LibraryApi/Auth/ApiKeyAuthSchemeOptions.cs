using Microsoft.AspNetCore.Authentication;

namespace MinimalApiTests.Filtering.LibraryApi.Auth;

public class ApiKeyAuthSchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = "VerySecret";
}