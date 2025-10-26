using Reto2_CleanHexagonal.Domain.Models;
using Reto2_CleanHexagonal.Domain.Ports;

namespace Reto2_CleanHexagonal.Application.Services
{
    /// <summary>
    /// Servicio de aplicación - Implementa la lógica de negocio y orquesta el flujo
    /// Implementa el puerto de entrada (IProductService)
    /// Depende del puerto de salida (IProductRepository)
    /// </summary> 
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID del producto no puede estar vacío", nameof(id));

            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetAvailableProductsAsync()
        {
            return await _productRepository.GetAvailableProductsAsync();
        }

        public async Task<Product> CreateProductAsync(string name, string description, decimal price, int stock)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del producto es requerido", nameof(name));

            if (price <= 0)
                throw new ArgumentException("El precio debe ser mayor a cero", nameof(price));

            if (stock < 0)
                throw new ArgumentException("El stock no puede ser negativo", nameof(stock));

            // Crear la entidad de dominio
            var product = new Product(name, description, price, stock);

            // Persistir usando el puerto de salida
            return await _productRepository.CreateAsync(product);
        }

        public async Task<Product> UpdateProductAsync(Guid id, string name, string description, decimal price)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID del producto no puede estar vacío", nameof(id));

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado");

            // Actualizar usando el método de dominio
            product.UpdateDetails(name, description, price);

            return await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> UpdateStockAsync(Guid id, int quantity)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID del producto no puede estar vacío", nameof(id));

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado");

            // Actualizar stock usando el método de dominio
            product.UpdateStock(quantity);

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID del producto no puede estar vacío", nameof(id));

            var exists = await _productRepository.ExistsAsync(id);
            if (!exists)
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado");

            return await _productRepository.DeleteAsync(id);
        }
    }
}
