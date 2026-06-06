using InfraEstrutura.Reporter.DataModels.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Sistema.Reporter.Server.Services;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // Escuta na porta 7191 aceitando apenas HTTP/2 (obrigatório para gRPC)
    options.ListenLocalhost(7237, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
        listenOptions.UseHttps(); // Remove se quiser testar sem SSL
    });
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("ConfigServer/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ConfigServer/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configuração do DbContext com PostgreSQL
builder.Services.AddDbContext<ReporterAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// Configura a conexão com o MongoDB
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb");
    return new MongoClient(mongoConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    var mongoDatabaseName = builder.Configuration["MongoDbSettings:DatabaseName"];
    return mongoClient.GetDatabase(mongoDatabaseName);
});


// Add services to the container.
builder.Services.AddGrpc();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<ReportService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
