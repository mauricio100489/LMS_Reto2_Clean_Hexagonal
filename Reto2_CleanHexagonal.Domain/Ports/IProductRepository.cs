using Reto2_CleanHexagonal.Domain.Models;

namespace Reto2_CleanHexagonal.Domain.Ports
{
    /// <summary>
    /// Puerto de salida (Output Port) - Define el contrato para persistencia de productos
    /// Los adaptadores de persistencia implementarán esta interfaz
    /// </summary>
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetAvailableProductsAsync();
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
