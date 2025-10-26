using Reto2_CleanHexagonal.Domain.Models;

namespace Reto2_CleanHexagonal.Domain.Ports
{
    /// <summary>
    /// Puerto de entrada (Input Port) - Define los casos de uso de la aplicación
    /// Los servicios de aplicación implementarán esta interfaz
    /// </summary>
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetAvailableProductsAsync();
        Task<Product> CreateProductAsync(string name, string description, decimal price, int stock);
        Task<Product> UpdateProductAsync(Guid id, string name, string description, decimal price);
        Task<bool> UpdateStockAsync(Guid id, int quantity);
        Task<bool> DeleteProductAsync(Guid id);
    }
}
