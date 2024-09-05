
using Microsoft.AspNetCore.HttpOverrides;

namespace Askstatus.Web.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
