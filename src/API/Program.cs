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
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configurar o DbContext com base no ambiente
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("TestingDb")); // Banco de dados em memória para testes
    Console.WriteLine("Executando no ambiente de testes com banco de dados em memória.");
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

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

    try
    {
        if (!builder.Environment.IsEnvironment("Testing")) // Evita migrações no ambiente de testes
        {
            Console.WriteLine($"Conectado ao banco de dados: {connectionString}");
            Console.WriteLine("Verificando e aplicando migrações pendentes...");
            dbContext.Database.Migrate(); // Aplica apenas as migrações pendentes
            Console.WriteLine("Migrações aplicadas com sucesso.");
        }
        else
        {
            Console.WriteLine("Banco de dados em memória configurado para testes.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao configurar o banco de dados: {ex.Message}");
        throw;
    }
}

var url = builder.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5000";
Console.WriteLine($"Aplicação iniciada e disponível em: {url}");

app.UseSerilogRequestLogging();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Torne a classe Program pública
public partial class Program { }