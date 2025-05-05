using Domain.Entities;

namespace Application.Services
{
    public class ValidationService
    {
        public void ValidateProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Codigo))
                throw new ArgumentException("O código do produto é obrigatório.");

            if (string.IsNullOrEmpty(product.Nome))
                throw new ArgumentException("O nome do produto é obrigatório.");

            if (product.Preco <= 0)
                throw new ArgumentException("O preço deve ser maior que zero.");
        }
    }
}