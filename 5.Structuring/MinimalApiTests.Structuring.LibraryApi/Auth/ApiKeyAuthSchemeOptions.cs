using Microsoft.AspNetCore.Authentication;

namespace MinimalApiTests.Structuring.LibraryApi.Auth;

public class ApiKeyAuthSchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = "VerySecret";
}