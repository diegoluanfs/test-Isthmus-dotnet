using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;
using Domain.DTOs;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly SanitizationService _sanitizationService;
        private readonly ValidationService _validationService;
        private readonly GuidService _guidService;
        private readonly IProductRepository _productRepository;

        public ProductService(
            SanitizationService sanitizationService,
            ValidationService validationService,
            GuidService guidService,
            IProductRepository productRepository)
        {
            _sanitizationService = sanitizationService;
            _validationService = validationService;
            _guidService = guidService;
            _productRepository = productRepository;
        }

        public async Task<Product> ConvertAndSanitizeProductAsync(ProductDto productDto)
        {
            // Converter ProductDto para Product
            var product = new Product
            {
                Codigo = productDto.Codigo,
                Nome = productDto.Nome,
                Descricao = productDto.Descricao,
                Preco = productDto.Preco,
                Ativo = productDto.Ativo
            };

            product.Id = _guidService.GenerateGuid();

            // Sanitizar os campos
            product.Codigo = _sanitizationService.SanitizeString(product.Codigo?.Trim().ToUpper());
            product.Nome = _sanitizationService.SanitizeString(product.Nome?.Trim());
            product.Descricao = _sanitizationService.SanitizeString(product.Descricao?.Trim());

            // Validar o produto
            _validationService.ValidateProduct(product);

            return product;
        }

        public async Task AddProductAsync(Product product)
        {
            if (await _productRepository.ExistsByCodigoAsync(product.Codigo))
            {
                throw new ArgumentException("Já existe um produto com o mesmo código.");
            }

            await _productRepository.AddAsync(product);
        }
    }
}