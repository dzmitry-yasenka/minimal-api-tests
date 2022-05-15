using FluentValidation;
using MinimalApiTests.Structuring.LibraryApi.Auth;
using MinimalApiTests.Structuring.LibraryApi.Data;
using MinimalApiTests.Structuring.LibraryApi.Endpoints.Internal;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args
    // ApplicationName = "Library.API"
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AnyOrigin", policyBuilder => policyBuilder.AllowAnyOrigin());
});

builder.Configuration.AddJsonFile(@"appsettings.Local.json", true, true);
Console.WriteLine(builder.Configuration.GetValue<string>("Section:SubSection"));


builder.Services.AddAuthentication(ApiKeySchemeConstants.SchemeName)
    .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(ApiKeySchemeConstants.SchemeName, _ => { });
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new SqliteConnectionFactory(builder.Configuration.GetValue<string>("Database:ConnectionString")));
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddEndpoints<Program>(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.UseCors();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.UseEndpoints<Program>();

// DB initialization here
var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

app.Run();