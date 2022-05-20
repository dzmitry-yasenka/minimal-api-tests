using FluentValidation;
using MinimalApiTests.Testing.LibraryApi.Auth;
using MinimalApiTests.Testing.LibraryApi.Data;
using MinimalApiTests.Testing.LibraryApi.Endpoints.Internal;

//configure the builder
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args
    // ApplicationName = "Library.API"
});

// configure services in the container
builder.Services.AddCors(options =>
{
    options.AddPolicy("AnyOrigin", policyBuilder => policyBuilder.AllowAnyOrigin());
});
builder.Configuration.AddJsonFile(@"appsettings.Local.json", true, true);
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

// configure middlewares
app.UseCors();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.UseEndpoints<Program>();

// DB initialization here
var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

// run Library API
app.Run();