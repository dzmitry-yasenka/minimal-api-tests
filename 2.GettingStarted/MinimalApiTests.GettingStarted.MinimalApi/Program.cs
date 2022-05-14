using System.Text;
using Microsoft.AspNetCore.Mvc;
using MinimalApiTests.GettingStarted.MinimalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<PeopleService>();
builder.Services.AddSingleton<GuidGenerator>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");
app.MapGet("get-example", () => "Hello from GET");
app.MapPost("post-example", () => "Hello from POST");
app.MapGet("ok-object", () => Results.Ok(new {Name = "SomeName"}));
app.MapGet("slow-ok-object", async () =>
{
    await Task.Delay(3000);
    return Results.Ok(new {Name = "SomeName"});
});

app.MapGet("get", () => "This is a GET");
app.MapPost("post", () => "This is a POST");
app.MapPut("put", () => "This is a PUT");
app.MapDelete("delete", () => "This is a DELETE");

app.MapMethods("options-or-head", new[] {"HEAD", "OPTIONS"}, 
    () => "This is a response from OPTIONS or HEAD");

var handler = () => "This is coming from a var";
app.MapGet("handler", handler);
app.MapGet("from-class", Example.SomeMethod);

app.MapGet("get-params/{age:int}", (int age) => $"Provided age was {age}");

app.MapGet("cars/{carId:regex(^[a-z0-9]+$)}", (string carId) => $"Provided car id was {carId}");
app.MapGet("books/{isbn:length(13)}", (string isbn) => $"Provided ISBN was {isbn}");

app.MapGet("people/search", (string? searchTerm, PeopleService peopleService) =>
{
    if (searchTerm is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(peopleService.Search(searchTerm));
});

app.MapGet("mix/{routeParam}", (
    [FromRoute] string routeParam, 
    [FromQuery(Name = "query")] string queryParam,
    [FromServices] GuidGenerator guidGenerator,
    [FromHeader(Name = "Accept-Encoding")] string encoding) => $"{routeParam} {queryParam} {guidGenerator.GetGuid()} {encoding}");

app.MapPost("people", (Person person) =>
{
    return Results.Ok(person);
});

app.MapGet("http-context-1", async context =>
{
    await context.Response.WriteAsync("Hello from http-context-1");
});

app.MapGet("http-context-2", async (HttpContext context) =>
{
    await context.Response.WriteAsync("Hello from http-context-2");
});

app.MapGet("http-context-3", async (HttpRequest request, HttpResponse response) =>
{
    await response.WriteAsync("Hello from http-context-3");
});

app.MapGet("http", async (HttpRequest request, HttpResponse response) =>
{
    var queries = request.QueryString.Value;
    await response.WriteAsync($"Queries were: {queries}");
});

app.MapGet("map-point", (MapPoint mapPoint) =>
{
    Console.WriteLine($"Lat: {mapPoint.Latitude} Long: {mapPoint.Longitude}");
    return Results.Ok(mapPoint); 
});

app.MapGet("simple-string", () => "simple string");
app.MapGet("simple-raw-json", () => new {Message = "simple raw json"});
app.MapGet("ok-obj", () => Results.Ok(new {Message = "ok-obj message"}));
app.MapGet("ok-json", () => Results.Json(new {Message = "ok-json message"}));
app.MapGet("text-string", () => Results.Text("Hello from text string"));

app.MapGet("stream-result", () =>
{
    var memoryStream = new MemoryStream();
    var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
    streamWriter.Write("Hello World!");
    streamWriter.Flush();
    memoryStream.Seek(0, SeekOrigin.Begin);
    return Results.Stream(memoryStream);
});

app.MapGet("redirect", () => Results.Redirect("https://www.google.com"));

app.MapGet("download", () => Results.File("./my-file.txt"));

app.MapGet("logging", (ILogger<Program> logger) =>
{
    logger.LogInformation("Logging information");
    return Results.Ok();
});

//app.Urls.Add("https://localhost:8888");
//app.Urls.Add("https://localhost:9999");
app.Run();