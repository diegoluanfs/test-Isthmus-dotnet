using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;
using Application.DTOs;

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
            // Verifica se já existe um produto com o mesmo código e ativo = true
            var existingProduct = await _productRepository.GetByCodigoAsync(product.Codigo);

            if (existingProduct != null && existingProduct.Ativo)
            {
                // Atualiza os campos do produto existente, mantendo o Id
                existingProduct.Nome = product.Nome;
                existingProduct.Descricao = product.Descricao;
                existingProduct.Preco = product.Preco;

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

        public async Task<bool> UpdateProductAsync(Product product)
        {
            // Verifica se existe algum produto com o mesmo código e ativo = true
            var conflictingProduct = await _productRepository.GetByCodigoAsync(product.Codigo);

            if (conflictingProduct != null && conflictingProduct.Id != product.Id && conflictingProduct.Ativo)
            {
                // Não é possível atualizar, pois já existe um produto ativo com o mesmo código
                return false;
            }

            var existingProduct = await _productRepository.GetByIdAsync(product.Id);

            if (existingProduct == null)
            {
                return false; // Produto não encontrado
            }

            // Atualiza os campos do produto existente
            existingProduct.Codigo = product.Codigo;
            existingProduct.Nome = product.Nome;
            existingProduct.Descricao = product.Descricao;
            existingProduct.Preco = product.Preco;
            existingProduct.Ativo = product.Ativo;

            await _productRepository.UpdateAsync(existingProduct);
            return true;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);

            if (existingProduct == null)
            {
                return false; // Produto não encontrado
            }

            // Exclusão lógica: marca o produto como inativo
            existingProduct.Ativo = false;

            await _productRepository.UpdateAsync(existingProduct);
            return true;
        }

    }
}