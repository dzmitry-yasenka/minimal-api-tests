using MinimalApiTests.LetsBuild.LibraryApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new SqliteConnectionFactory(builder.Configuration.GetValue<string>("Database:ConnectionString")));
builder.Services.AddSingleton<DatabaseInitializer>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// DB initialization here
var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

app.Run();