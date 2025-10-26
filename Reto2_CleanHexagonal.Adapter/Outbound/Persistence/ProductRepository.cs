using Reto2_CleanHexagonal.Domain.Models;
using Reto2_CleanHexagonal.Domain.Ports;

namespace Reto2_CleanHexagonal.Adapter.Outbound.Persistence
{
    /// <summary>
    /// Adaptador de salida (Outbound Adapter) - Implementación en memoria del repositorio
    /// Implementa el puerto de salida IProductRepository
    /// En producción, esto podría ser Entity Framework, Dapper, etc.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new();
        private readonly object _lock = new();

        public Task<Product?> GetByIdAsync(Guid id)
        {
            lock (_lock)
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                return Task.FromResult(product);
            }
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            lock (_lock)
            {
                return Task.FromResult<IEnumerable<Product>>(_products.ToList());
            }
        }

        public Task<IEnumerable<Product>> GetAvailableProductsAsync()
        {
            lock (_lock)
            {
                var availableProducts = _products.Where(p => p.IsAvailable()).ToList();
                return Task.FromResult<IEnumerable<Product>>(availableProducts);
            }
        }

        public Task<Product> CreateAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            lock (_lock)
            {
                _products.Add(product);
                return Task.FromResult(product);
            }
        }

        public Task<Product> UpdateAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            lock (_lock)
            {
                var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
                if (existingProduct == null)
                    throw new KeyNotFoundException($"Producto con ID {product.Id} no encontrado");

                var index = _products.IndexOf(existingProduct);
                _products[index] = product;

                return Task.FromResult(product);
            }
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            lock (_lock)
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                    return Task.FromResult(false);

                _products.Remove(product);
                return Task.FromResult(true);
            }
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            lock (_lock)
            {
                var exists = _products.Any(p => p.Id == id);
                return Task.FromResult(exists);
            }
        }
    }
}
