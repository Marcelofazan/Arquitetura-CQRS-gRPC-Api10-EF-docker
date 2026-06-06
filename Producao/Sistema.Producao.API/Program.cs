using InfraEstrutura.Producao.DataModels.Data;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using InfraEstrutura.Producao.Business;
using InfraEstrutura.Producao.Business.CommandHandlers;
using Sistema.Producao.Sdk.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os controladores (Web API)
builder.Services.AddControllers();

//// Configura��o de DbContext com InMemory
//builder.Services.AddDbContext<LancamentoAppDbContext>(options =>
//    options.UseInMemoryDatabase("LancamentoDb"));


// Configurar o PostgreSQL
builder.Services.AddDbContext<ProducaoAppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgresConnection"),
        npgsqlOptions => npgsqlOptions.MigrationsAssembly("InfraEstrutura.Producao.DataModels")
    )
);

// Configura��o do MediatR para os Command Handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(RegistrarClienteHandler).Assembly,
    typeof(ApagarClienteHandler).Assembly,
    typeof(AtualizarClienteHandler).Assembly
));

builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new ConnectionFactory()
    {
        HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost",
        UserName = builder.Configuration["RabbitMQ:UserName"] ?? "guest",
        Password = builder.Configuration["RabbitMQ:Password"] ?? "guest"
    };
});

builder.Services.AddSingleton<RabbitMQPublisher>();

/* builder.Services.AddSingleton<IConnectionFactory>(sp => 
{
    return new ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost",
        UserName = builder.Configuration["RabbitMQ:UserName"] ?? "guest",
        Password = builder.Configuration["RabbitMQ:Password"] ?? "guest"
    };
});

// Registra o seu publicador
builder.Services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>(); */

builder.Services.AddGrpcSdk();
builder.Services.AddGrpc();

// Swagger/OpenAPI para documenta��o
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAI Agent API V1"));
}

app.UseCors(builder =>
    builder.WithOrigins("http://localhost:3000")
           .AllowAnyMethod()
           .AllowAnyHeader());

//Migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProducaoAppDbContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Mapeia os controladores de API
app.MapControllers();

//// Mapeia o servi�o gRPC (LauchService)
//app.MapGrpcService<LauchService>();

app.Run();
