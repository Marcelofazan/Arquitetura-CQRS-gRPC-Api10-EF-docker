using MongoDB.Driver;
using RabbitMQ.Client;
using Sistema.Reporter.Sdk.Extensions;
using Sistema.Reporter.Business.CommandHandlers;
using Sistema.Reporter.Business.Interface;
using Sistema.Reporter.Business.Service;
using Sistema.Reporter.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Configs/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"Configs/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


// Add services to the container.
builder.Services.AddControllers();

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

builder.Services.AddScoped<IRelatorioGrpcService, RelatorioGrpcService>();
builder.Services.AddScoped<IRelatorioMongoService, RelatorioMongoService>();

// Configuração do MediatR para os Command Handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(ConsolidacaoDiariaCommandHandler).Assembly
));

builder.Services.AddSingleton<RabbitMQConsumer>();
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();


builder.Services.AddScoped<ConsolidacaoDiariaCommandHandler>();

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


// Configuração do gRPC SDK e serviços
builder.Services.AddGrpcSdk();
builder.Services.AddGrpc();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.UseHttpsRedirection();
app.UseAuthorization();

// Mapear controladores (APIs HTTP)
app.MapControllers();

// Mapear serviços gRPC
app.MapGrpcService<ReportService>();

// Inicia o consumo de mensagens RabbitMQ 
var rabbitMQService = app.Services.GetRequiredService<IRabbitMQService>();

rabbitMQService.StartListening("queue_consolidacao_diaria", async message =>
{
    // Cria um novo escopo para cada mensagem recebida 
    using (var scope = app.Services.CreateScope())
    {
        var handler = scope.ServiceProvider.GetRequiredService<ConsolidacaoDiariaCommandHandler>();
        await handler.ProcessMessageAsync(message); // Processa a mensagem recebida 
    }
});


app.Run();
