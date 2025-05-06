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

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
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
            // Verifica se o produto já existe pelo código
            var existingProduct = await _productRepository.GetByCodigoAsync(product.Codigo);

            if (existingProduct != null)
            {
                // Atualiza os campos do produto existente, mantendo o Id
                existingProduct.Nome = product.Nome;
                existingProduct.Descricao = product.Descricao;
                existingProduct.Preco = product.Preco;
                existingProduct.Ativo = product.Ativo;

                // Atualiza o produto no repositório
                await _productRepository.UpdateAsync(existingProduct);

                // Atualiza o objeto `product` com o Id do produto existente
                product.Id = existingProduct.Id;
            }
            else
            {
                // Gera um novo Id para o produto
                product.Id = _guidService.GenerateGuid();

                // Adiciona o novo produto
                await _productRepository.AddAsync(product);
            }
        }
    }
}