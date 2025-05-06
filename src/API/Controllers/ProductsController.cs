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


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();

                if (products == null || !products.Any())
                {
                    return NoContent(); // Retorna 204 se não houver produtos
                }

                return Ok(products); // Retorna 200 com a lista de produtos
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductDto productDto)
        {
            try
            {
                if (id == Guid.Empty || 
                    string.IsNullOrWhiteSpace(productDto.Codigo) ||
                    string.IsNullOrWhiteSpace(productDto.Nome) ||
                    string.IsNullOrWhiteSpace(productDto.Descricao) ||
                    productDto.Preco <= 0)
                {
                    return BadRequest("Dados inválidos.");
                }

                var product = await _productService.ConvertAndSanitizeProductAsync(productDto);
                product.Id = id; // Define o ID do produto a ser atualizado

                var updated = await _productService.UpdateProductAsync(product);

                if (!updated)
                {
                    return NotFound($"Produto com ID {id} não encontrado.");
                }

                return NoContent(); // Retorna 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("ID inválido.");
                }

                var deleted = await _productService.DeleteProductAsync(id);

                if (!deleted)
                {
                    return NotFound($"Produto com ID {id} não encontrado.");
                }

                return NoContent(); // Retorna 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
        
    }
}