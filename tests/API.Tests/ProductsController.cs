using System.Net;
using System.Net.Http.Json;
using API;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace API.Tests
{
    public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;

        public ProductsControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _serviceProvider = factory.Services;
        }

        private void ClearDatabase()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Products.RemoveRange(context.Products);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            ClearDatabase(); // Limpa o banco antes do teste

            // Act
            var response = await _client.GetAsync("/api/Products");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_AddsProduct_ReturnsCreated()
        {
            ClearDatabase(); // Limpa o banco antes do teste

            // Arrange
            var product = new Product
            {
                Codigo = "P001",
                Nome = "Produto Teste",
                Descricao = "Descrição do Produto Teste",
                Preco = 100.50m,
                Ativo = true
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Products", product);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}