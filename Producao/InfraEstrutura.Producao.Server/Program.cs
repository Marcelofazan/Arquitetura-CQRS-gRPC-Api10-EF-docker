using InfraEstrutura.Producao.Server.Services;
using InfraEstrutura.Producao.DataModels.Data;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Configura��o de DbContext com InMemory
//builder.Services.AddDbContext<LancamentoAppDbContext>(options =>
//    options.UseInMemoryDatabase("LancamentoDb"));

builder.WebHost.ConfigureKestrel(options =>
{
    // Escuta na porta 7191 aceitando apenas HTTP/2 (obrigatório para gRPC)
    options.ListenLocalhost(7026, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
        listenOptions.UseHttps(); // Remove se quiser testar sem SSL
    });
});

// Configura o PostgreSQL no Entity Framework
builder.Services.AddDbContext<ProducaoAppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgresConnection"),
        npgsqlOptions => npgsqlOptions.MigrationsAssembly("InfraEstrutura.Producao.DataModels") // Nome correto do projeto que cont�m o DbContext
    )
);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ClienteService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
