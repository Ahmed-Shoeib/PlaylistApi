using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlaylistApi.Data;

namespace PlaylistApi.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public CustomWebApplicationFactory()
    {
        // An in-memory SQLite database exists only while its connection is open.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Completely remove the production SQL Server DbContext registration.
            services.RemoveAll<
                IDbContextOptionsConfiguration<PlaylistDbContext>>();

            services.RemoveAll<
                DbContextOptions<PlaylistDbContext>>();

            services.RemoveAll<PlaylistDbContext>();

            // Register SQLite in-memory for integration tests.
            services.AddDbContext<PlaylistDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // Create the SQLite database schema.
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();

            var dbContext =
                scope.ServiceProvider.GetRequiredService<PlaylistDbContext>();

            dbContext.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection.Close();
            _connection.Dispose();
        }

        base.Dispose(disposing);
    }
}
