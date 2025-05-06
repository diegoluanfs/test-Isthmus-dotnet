using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar o Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Lê as configurações do appsettings.json
    .Enrich.FromLogContext()
    .WriteTo.Console() // Log no console
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Log em arquivo
    .CreateLogger();

builder.Host.UseSerilog(); // Substitui o logger padrão pelo Serilog

// Configurar o DbContext com base no ambiente
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("TestingDb"));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Adicionar serviços ao container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<SanitizationService>();
builder.Services.AddScoped<ValidationService>();
builder.Services.AddScoped<GuidService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var connectionString = dbContext.Database.GetDbConnection().ConnectionString;
    Console.WriteLine($"Conectado ao banco de dados: {connectionString}");

    if (app.Environment.IsDevelopment())
    {
        Console.WriteLine("Iniciando migrações...");
        dbContext.Database.Migrate();
        Console.WriteLine("Migrações concluídas.");
    }
}

var url = builder.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5000";
Console.WriteLine($"Aplicação iniciada e disponível em: {url}");

app.UseSerilogRequestLogging(); // Adiciona logs para cada requisição

app.UseAuthorization();
app.MapControllers();
app.Run();

// Torne a classe Program pública
public partial class Program { }