﻿using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MinimalApiTests.Testing.LibraryApi.Data;

namespace MinimalApiTests.Testing.LibraryApi.Tests.Integration;

public class LibraryApiFactory : WebApplicationFactory<IApiMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IDbConnectionFactory));
            services.AddSingleton<IDbConnectionFactory>(_ =>
                new SqliteConnectionFactory("DataSource=file:inmem?mode=memory&cache=shared"));
        });
    }
}