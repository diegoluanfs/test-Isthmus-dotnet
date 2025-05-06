using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Adicionar o contexto do banco de dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adicionar serviços do Swagger
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

    dbContext.Database.EnsureDeleted(); // Exclui o banco de dados, se existir
    dbContext.Database.Migrate(); // Aplica as migrações e recria o banco de dados
}

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("TestingDb"));
}

app.UseAuthorization();
app.MapControllers();
app.Run();

// Torne a classe Program pública
public partial class Program { }