using Microsoft.AspNetCore.Mvc;
using Domain.DTOs;
using Domain.Entities;
using Application.Interfaces;
using API.Controllers;
using Moq;
using Xunit;
using AutoFixture;

namespace API.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly ProductsController _controller;
        private readonly Fixture _fixture;

        public ProductsControllerTests()
        {
            _productServiceMock = new Mock<IProductService>();
            _controller = new ProductsController(_productServiceMock.Object);
            _fixture = new Fixture(); // Usado para gerar dados automaticamente
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenProductIsValid()
        {
            // Arrange
            var productDto = _fixture.Create<ProductDto>();
            var product = _fixture.Create<Product>();

            _productServiceMock
                .Setup(service => service.ConvertAndSanitizeProductAsync(productDto))
                .ReturnsAsync(product);

            _productServiceMock
                .Setup(service => service.AddProductAsync(product))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(productDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(product, createdResult.Value);
        }

        [Fact]
        public async Task Create_ShouldReturnConflict_WhenProductWithSameCodigoExists()
        {
            // Arrange
            var productDto = _fixture.Create<ProductDto>();

            _productServiceMock
                .Setup(service => service.ConvertAndSanitizeProductAsync(productDto))
                .ThrowsAsync(new ArgumentException("Já existe um produto com o mesmo código."));

            // Act
            var result = await _controller.Create(productDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Já existe um produto com o mesmo código.", conflictResult.Value);
        }

        [Fact]
        public async Task Create_ShouldReturnInternalServerError_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            var productDto = _fixture.Create<ProductDto>();

            _productServiceMock
                .Setup(service => service.ConvertAndSanitizeProductAsync(productDto))
                .ThrowsAsync(new Exception("Erro inesperado."));

            // Act
            var result = await _controller.Create(productDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erro interno: Erro inesperado.", statusCodeResult.Value);
        }

        [Theory]
        [InlineData(null, "Nome válido", "Descrição válida", 100.50, true)] // Código nulo
        [InlineData("P001", "", "Descrição válida", 100.50, true)] // Nome vazio
        [InlineData("P001", "Nome válido", "", 100.50, true)] // Descrição vazia
        [InlineData("P001", "Nome válido", "Descrição válida", -10, true)] // Preço negativo
        public async Task Create_ShouldReturnBadRequest_WhenProductDtoIsInvalid(
            string codigo, string nome, string descricao, decimal preco, bool ativo)
        {
            // Arrange
            var productDto = new ProductDto
            {
                Codigo = codigo,
                Nome = nome,
                Descricao = descricao,
                Preco = preco,
                Ativo = ativo
            };

            // Act
            var result = await _controller.Create(productDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ShouldCallConvertAndSanitizeProductAsync_Once()
        {
            // Arrange
            var productDto = _fixture.Create<ProductDto>();
            var product = _fixture.Create<Product>();

            _productServiceMock
                .Setup(service => service.ConvertAndSanitizeProductAsync(productDto))
                .ReturnsAsync(product);

            // Act
            await _controller.Create(productDto);

            // Assert
            _productServiceMock.Verify(service => service.ConvertAndSanitizeProductAsync(productDto), Times.Once);
        }
    }
}