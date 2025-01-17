using Askstatus.Application;
using Askstatus.Application.Interfaces;
using Askstatus.Infrastructure;
using Askstatus.Infrastructure.Data;
using Askstatus.Infrastructure.Hubs;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Askstatus.Web.API;

public class Program
{
    public static bool IsIntegrationTestRun = false;

    private const string SeedArgs = "--seed";
    private const string SerilogOutputTemplate = "[{Timestamp:HH:mm:ss} {SourceContext} [{Level}] {Message}{NewLine}{Exception}";
    //private const string SerilogOutputTemplate = "[{Timestamp:HH:mm:ss} {SourceContext} [{Level}] CLient IP: {ClientIp} {Message}{NewLine}{Exception}";

    private static IConfiguration GetConfiguration(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .AddUserSecrets("8b69e137-e330-4e6f-b0ab-9afb3de16f6f")
            .AddCommandLine(args)
            .Build();
        return configuration;
    }


    public static async Task<int> Main(string[] args)
    {
        int res = 0;
        if (!IsIntegrationTestRun)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(GetConfiguration(args))
                .Enrich.FromLogContext()
                //.Enrich.WithClientIp()
                .WriteTo.Console(outputTemplate: SerilogOutputTemplate)
                .CreateBootstrapLogger();
        }
        try
        {
            var applyDbMigrationWithDataSeedFromProgramArguments = args.Any(x => x == SeedArgs);
            if (applyDbMigrationWithDataSeedFromProgramArguments)
            {
                try
                {
                    Log.ForContext<Program>().Information("Migrating and Seeding data");
                    await SeedData(args);
                    Log.ForContext<Program>().Information("Migrating and Seeding data done");
                }
                catch (Exception ex)
                {
                    Log.ForContext<Program>().Error(ex, "An error occurred while migrating or initializing the database.");
                    return 1;
                }
                return 0;
            }

            Log.ForContext<Program>().Information("Starting web host");

            WebApplicationBuilder builder = CreateBuilder(args);
            WebApplication app = CreateWebApp(builder);

            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;
            var addressService = services.GetRequiredService<IApplicationHostAddressService>();
            var ipAddress = addressService.IpAddress;
            Log.ForContext<Program>().Information("Application host address: {ipAddress}", ipAddress);
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
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
        builder.Services.AddSerilog((services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                //.Enrich.WithClientIp()
                .WriteTo.Console(outputTemplate: SerilogOutputTemplate)
;
        });

        builder.Services.AddInfrastructureServices(builder.Environment, builder.Configuration.GetConnectionString("DefaultConnection")!);
        builder.Services.AddApplicationServices();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // Add a CORS policy for the client
        builder.Services.AddCors(
            options => options.AddPolicy(
                "wasm",
                policy => policy.WithOrigins([
                    builder.Configuration["BackendUrl"] ?? "https://localhost:7298",
                    builder.Configuration["FrontendUrl"] ?? "https://localhost:7117"]
                    )
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

        app.UseExceptionHandler();

        //app.UseHttpsRedirection();
        // Activate the CORS policy
        app.UseCors("wasm");

        // Enable authentication and authorization after CORS Middleware
        // processing (UseCors) in case the Authorization Middleware tries
        // to initiate a challenge before the CORS Middleware has a chance
        // to set the appropriate headers.
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();
        app.MapHub<StatusHub>("/statushub");
        return app;
    }

    private static async Task SeedData(string[] args)
    {
        var configuration = GetConfiguration(args);
        var password = configuration["SeedPassword"];
        var sqliteBuilder = new SqliteConnectionStringBuilder(configuration.GetConnectionString("DefaultConnection"));
        if (!IsFullPath(sqliteBuilder.DataSource))
        {
            sqliteBuilder.DataSource = Path.Combine(Directory.GetCurrentDirectory(), sqliteBuilder.DataSource);
        }
        var directory = Path.GetDirectoryName(sqliteBuilder.DataSource);
        Log.ForContext<Program>().Information("Databese path {path}", directory);

        if (!Directory.Exists(directory))
        {
            Log.ForContext<Program>().Information("Databese path {path} does not exist", directory);
            Directory.CreateDirectory(directory!);
            Log.ForContext<Program>().Information("Databese path {path} created", directory);
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));

        await using var dbContext = new ApplicationDbContext(optionsBuilder.Options);
        var initializer = new DbInitializer(dbContext);
        await initializer.SeedAsync(password!);
    }

    private static bool IsFullPath(string path)
    {
        return !String.IsNullOrWhiteSpace(path)
            && path.IndexOfAny(System.IO.Path.GetInvalidPathChars().ToArray()) == -1
            && Path.IsPathRooted(path)
            && !Path.GetPathRoot(path)?.Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) == true;
    }
}
