using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using Azure.Identity;
using MailKit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using N9.Api.Apis;
using N9.Api.Extensions;
using N9.Data.Context;
using N9.Services.Models;
using Polly;
using Polly.Retry;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddConfig();

builder.Services.AddProblemDetails();

// Add Http logging
builder.Services.AddHttpLogging(options => { options.LoggingFields = HttpLoggingFields.All; });

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

// Add Polly
builder.Services.AddResiliencePipeline("default",
    rp =>
    {
        rp.AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 4,
            Delay = TimeSpan.FromMicroseconds(1000),
            ShouldHandle = new PredicateBuilder().Handle<Exception>()
        });
    });

// Add Http Client
builder.Services.AddHttpClient("default").AddStandardResilienceHandler();

// Add Cors
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApiWithSecurityScheme();

// Add Azure Key Vault
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}

builder.Services
    .AddDbContext<BooksDbContext>(options =>
        options
            .UseSqlServer(builder.Configuration.GetConnectionString("Sql"))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

var app = builder.Build();

// Error handling
app.UseStatusCodePages(async statusCodeContext 
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
        .ExecuteAsync(statusCodeContext.HttpContext));

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(a => { a.Run(async (ctx) => await Results.Problem().ExecuteAsync(ctx)); });
}

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithEndpointPrefix("/api-docs/{documentName}");
});

app.UseHttpLogging();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Map routes
app.MapBooksApi();

app.Run();