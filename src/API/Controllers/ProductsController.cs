using Microsoft.AspNetCore.Mvc;
using Domain.DTOs;
using Application.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService; // Alterado para IProductService

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productDto.Codigo) ||
                    string.IsNullOrWhiteSpace(productDto.Nome) ||
                    string.IsNullOrWhiteSpace(productDto.Descricao) ||
                    productDto.Preco <= 0)
                {
                    return BadRequest("Dados inválidos.");
                }

                var product = await _productService.ConvertAndSanitizeProductAsync(productDto);
                await _productService.AddProductAsync(product);

                return CreatedAtAction(nameof(Create), new { id = product.Id }, product);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}