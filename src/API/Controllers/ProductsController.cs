using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Lista todos os produtos.
        /// </summary>
        /// <returns>Uma lista de produtos.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os produtos", Description = "Retorna uma lista de produtos cadastrados.")]
        [SwaggerResponse(200, "Lista de produtos retornada com sucesso.")]
        [SwaggerResponse(204, "Nenhum produto encontrado.")]
        [SwaggerResponse(500, "Erro interno.")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();

                if (products == null || !products.Any())
                {
                    return Ok(new ApiResponse<object>(204, "Nenhum produto encontrado."));
                }

                return Ok(new ApiResponse<IEnumerable<Product>>(200, "Lista de produtos retornada com sucesso.", products));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(500, "Erro interno.", null, new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Cria um novo produto.
        /// </summary>
        /// <param name="productDto">Dados do produto a ser criado.</param>
        /// <returns>O produto criado.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo produto", Description = "Adiciona um novo produto ao sistema.")]
        [SwaggerResponse(201, "Produto criado com sucesso.")]
        [SwaggerResponse(400, "Dados inválidos.")]
        [SwaggerResponse(500, "Erro interno.")]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productDto.Codigo) ||
                    string.IsNullOrWhiteSpace(productDto.Nome) ||
                    string.IsNullOrWhiteSpace(productDto.Descricao) ||
                    productDto.Preco <= 0)
                {
                    return BadRequest(new ApiResponse<object>(400, "Dados inválidos."));
                }

                var product = await _productService.ConvertAndSanitizeProductAsync(productDto);
                await _productService.AddProductAsync(product);

                return CreatedAtAction(nameof(Create), new { id = product.Id },
                    new ApiResponse<Product>(201, "Produto criado com sucesso.", product));
            }
            catch (ArgumentException ex)
            {
                return Conflict(new ApiResponse<object>(409, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(500, "Erro interno.", null, new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        /// <param name="id">ID do produto a ser atualizado.</param>
        /// <param name="productDto">Dados atualizados do produto.</param>
        /// <returns>Status da operação.</returns>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza um produto", Description = "Atualiza os dados de um produto existente.")]
        [SwaggerResponse(204, "Produto atualizado com sucesso.")]
        [SwaggerResponse(400, "Dados inválidos.")]
        [SwaggerResponse(404, "Produto não encontrado.")]
        [SwaggerResponse(500, "Erro interno.")]
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
                    return BadRequest(new ApiResponse<object>(400, "Dados inválidos."));
                }

                var product = await _productService.ConvertAndSanitizeProductAsync(productDto);
                product.Id = id;

                var updated = await _productService.UpdateProductAsync(product);

                if (!updated)
                {
                    return NotFound(new ApiResponse<object>(404, $"Produto com ID {id} não encontrado."));
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(500, "Erro interno.", null, new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Exclui um produto.
        /// </summary>
        /// <param name="id">ID do produto a ser excluído.</param>
        /// <returns>Status da operação.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Exclui um produto", Description = "Remove logicamente um produto do sistema.")]
        [SwaggerResponse(204, "Produto excluído com sucesso.")]
        [SwaggerResponse(400, "ID inválido.")]
        [SwaggerResponse(404, "Produto não encontrado.")]
        [SwaggerResponse(500, "Erro interno.")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>(400, "ID inválido."));
                }

                var deleted = await _productService.DeleteProductAsync(id);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<object>(404, $"Produto com ID {id} não encontrado."));
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(500, "Erro interno.", null, new List<string> { ex.Message }));
            }
        }
    }
}