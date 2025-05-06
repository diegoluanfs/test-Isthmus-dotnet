using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Domain.Entities;
using Application.Interfaces;
using API.Controllers;
using Moq;
using Xunit;
using AutoFixture;
using Xunit.Abstractions;

namespace API.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly ProductsController _controller;
        private readonly Fixture _fixture;
        private readonly ITestOutputHelper _output;

        public ProductsControllerTests(ITestOutputHelper output)
        {
            _productServiceMock = new Mock<IProductService>();
            _controller = new ProductsController(_productServiceMock.Object);
            _fixture = new Fixture(); // Usado para gerar dados automaticamente
            _output = output; // Captura o helper de saída
        }

        // Testes para o método Create
        [Fact]
        public async Task Create_ShouldReturnCreated_WhenProductIsValid()
        {
            _output.WriteLine("✅ Teste 1: Criação de produto quando é válido");

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
            var apiResponse = Assert.IsType<ApiResponse<Product>>(createdResult.Value);
            Assert.Equal(product, apiResponse.Data);
            Assert.Equal(201, apiResponse.Code);
            Assert.Equal("Produto criado com sucesso.", apiResponse.Message);

            _output.WriteLine("✅ Teste 1 concluído com sucesso!");
        }

        [Fact]
        public async Task Create_ShouldReturnConflict_WhenProductWithSameCodigoExists()
        {
            _output.WriteLine("🆘 Teste 2: Teste quando o produto apresenta o mesmo código");

            // Arrange
            var productDto = _fixture.Create<ProductDto>();

            _productServiceMock
                .Setup(service => service.ConvertAndSanitizeProductAsync(productDto))
                .ThrowsAsync(new ArgumentException("Já existe um produto com o mesmo código."));

            // Act
            var result = await _controller.Create(productDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(conflictResult.Value);
            Assert.Equal(409, apiResponse.Code);
            Assert.Equal("Já existe um produto com o mesmo código.", apiResponse.Message);

            _output.WriteLine("🆘 Teste 2 concluído com sucesso!");
        }

        [Fact]
        public async Task Create_ShouldReturnInternalServerError_WhenUnexpectedErrorOccurs()
        {
            _output.WriteLine("🆘 Teste 3: Teste quando ocorre um erro inesperado");

            // Arrange
            var productDto = _fixture.Create<ProductDto>();

            _productServiceMock
                .Setup(service => service.ConvertAndSanitizeProductAsync(productDto))
                .ThrowsAsync(new Exception("Erro inesperado."));

            // Act
            var result = await _controller.Create(productDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(statusCodeResult.Value);
            Assert.Equal(500, apiResponse.Code);
            Assert.Equal("Erro interno.", apiResponse.Message);
            Assert.Contains("Erro inesperado.", apiResponse.Errors);

            _output.WriteLine("🆘 Teste 3 concluído com sucesso!");
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenProductsExist()
        {
            _output.WriteLine("✅ Teste 4: Retorna produtos quando existem no banco");

            // Arrange
            var products = _fixture.CreateMany<Product>(5).ToList();

            _productServiceMock
                .Setup(service => service.GetAllProductsAsync())
                .ReturnsAsync(products);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<Product>>>(okResult.Value);
            Assert.Equal(products, apiResponse.Data);
            Assert.Equal(200, apiResponse.Code);
            Assert.Equal("Lista de produtos retornada com sucesso.", apiResponse.Message);

            _output.WriteLine("✅ Teste 4 concluído com sucesso!");
        }

        [Fact]
        public async Task GetAll_ShouldReturnNoContent_WhenNoProductsExist()
        {
            _output.WriteLine("🆘 Teste 5: Retorna nenhum conteúdo quando não há produtos");

            // Arrange
            _productServiceMock
                .Setup(service => service.GetAllProductsAsync())
                .ReturnsAsync(new List<Product>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal(204, apiResponse.Code);
            Assert.Equal("Nenhum produto encontrado.", apiResponse.Message);

            _output.WriteLine("🆘 Teste 5 concluído com sucesso!");
        }

        // Testes para o método Update
        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenProductIsUpdated()
        {
            _output.WriteLine("✅ Teste 6: Atualização de produto bem-sucedida");

            // Arrange
            var productDto = _fixture.Create<ProductDto>();
            var product = _fixture.Create<Product>();

            _productServiceMock
                .Setup(service => service.ConvertAndSanitizeProductAsync(productDto))
                .ReturnsAsync(product);

            _productServiceMock
                .Setup(service => service.UpdateProductAsync(product))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(product.Id, productDto);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _output.WriteLine("✅ Teste 6 concluído com sucesso!");
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            _output.WriteLine("🆘 Teste 7: Atualização falha quando o produto não existe");

            // Arrange
            var productDto = _fixture.Create<ProductDto>();
            var product = _fixture.Create<Product>();

            _productServiceMock
                .Setup(service => service.ConvertAndSanitizeProductAsync(productDto))
                .ReturnsAsync(product);

            _productServiceMock
                .Setup(service => service.UpdateProductAsync(product))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Update(product.Id, productDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal(404, apiResponse.Code);
            Assert.Equal($"Produto com ID {product.Id} não encontrado.", apiResponse.Message);

            _output.WriteLine("🆘 Teste 7 concluído com sucesso!");
        }

        // Testes para o método Delete
        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenProductIsDeleted()
        {
            _output.WriteLine("✅ Teste 8: Exclusão de produto bem-sucedida");

            // Arrange
            var productId = Guid.NewGuid();

            _productServiceMock
                .Setup(service => service.DeleteProductAsync(productId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(productId);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _output.WriteLine("✅ Teste 8 concluído com sucesso!");
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            _output.WriteLine("🆘 Teste 9: Exclusão falha quando o produto não existe");

            // Arrange
            var productId = Guid.NewGuid();

            _productServiceMock
                .Setup(service => service.DeleteProductAsync(productId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(productId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal(404, apiResponse.Code);
            Assert.Equal($"Produto com ID {productId} não encontrado.", apiResponse.Message);

            _output.WriteLine("🆘 Teste 9 concluído com sucesso!");
        }
    }
}