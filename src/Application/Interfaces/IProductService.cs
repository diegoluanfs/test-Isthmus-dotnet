using Domain.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<Product> ConvertAndSanitizeProductAsync(ProductDto productDto);
        Task AddProductAsync(Product product);
    }
}