using Askstatus.Application;
using Askstatus.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;

namespace Askstatus.Web.API;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddInfrastructureServices();
        builder.Services.AddApplicationServices();
        //builder.Services.AddProblemDetails();

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

        var app = builder.Build();

        //app.UseStatusCodePages();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        // Seed the database
        await using var scope = app.Services.CreateAsyncScope();
        await SeedData.InitializeAsync(scope.ServiceProvider);

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

        await app.RunAsync();
        return 0;
    }
}
