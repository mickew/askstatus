using Askstatus.Application;
using Askstatus.Infrastructure;
using Askstatus.Infrastructure.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Askstatus.Web.API;

public class Program
{
    private const string SeedArgs = "--seed";

    public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
      .AddEnvironmentVariables()
      .AddUserSecrets("8b69e137-e330-4e6f-b0ab-9afb3de16f6f")
      .Build();

    public static async Task<int> Main(string[] args)
    {
        int res = 0;
        // create logger

        try
        {
            //Log.Information("Starting web host");
            var applyDbMigrationWithDataSeedFromProgramArguments = args.Any(x => x == SeedArgs);
            if (applyDbMigrationWithDataSeedFromProgramArguments)
            {
                try
                {
                    //Log.Information("Migrating and Seeding data");
                    await SeedData();
                    //Log.Information("Migrating and Seeding data done");
                }
                catch (Exception)
                //catch (Exception ex)
                {
                    //Log.Error(ex, "An error occurred while migrating or initializing the database.");
                    return 1;
                }
                return 0;
            }
            WebApplicationBuilder builder = CreateBuilder(args);
            WebApplication app = CreateWebApp(builder);

            await app.RunAsync();
        }
        catch (Exception)
        //catch (Exception ex)
        {
            //Log.Fatal(ex, "Host terminated unexpectedly");
            res = 1;
        }
        finally
        {
            //Log.CloseAndFlush();
        }

        return res;
    }

    private static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddInfrastructureServices(builder.Environment, builder.Configuration.GetConnectionString("DefaultConnection")!);
        builder.Services.AddApplicationServices();

        // Add services to the container.
        // Add a CORS policy for the client
        builder.Services.AddCors(
            options => options.AddPolicy(
                "wasm",
                policy => policy.WithOrigins([builder.Configuration["BackendUrl"] ?? "https://localhost:7298",
            builder.Configuration["FrontendUrl"] ?? "https://localhost:7117"])
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));


        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        return builder;
    }

    private static WebApplication CreateWebApp(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseHttpsRedirection();
        // Activate the CORS policy
        app.UseCors("wasm");

        // Enable authentication and authorization after CORS Middleware
        // processing (UseCors) in case the Authorization Middleware tries
        // to initiate a challenge before the CORS Middleware has a chance
        // to set the appropriate headers.
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        return app;
    }

    private static async Task SeedData()
    {
        var sqliteBuilder = new SqliteConnectionStringBuilder(Configuration.GetConnectionString("DefaultConnection"));
        if (!IsFullPath(sqliteBuilder.DataSource))
        {
            sqliteBuilder.DataSource = Path.Combine(Directory.GetCurrentDirectory(), sqliteBuilder.DataSource);
        }
        var directory = Path.GetDirectoryName(sqliteBuilder.DataSource);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationBaseDbContext>();
        var s = Configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));

        await using var dbContext = new ApplicationBaseDbContext(optionsBuilder.Options);
        var initializer = new DbInitializer(dbContext);
        await initializer.SeedAsync();
    }

    private static bool IsFullPath(string path)
    {
        return !String.IsNullOrWhiteSpace(path)
            && path.IndexOfAny(System.IO.Path.GetInvalidPathChars().ToArray()) == -1
            && Path.IsPathRooted(path)
            && !Path.GetPathRoot(path)?.Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) == true;
    }
}
