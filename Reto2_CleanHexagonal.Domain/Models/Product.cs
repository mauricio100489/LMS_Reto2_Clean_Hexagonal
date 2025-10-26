namespace Reto2_CleanHexagonal.Domain.Models
{
    /// <summary>
    /// Entidad de dominio Product - Representa un producto en el sistema
    /// </summary>
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Product() { } // Constructor privado para EF Core

        public Product(string name, string description, decimal price, int stock)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del producto no puede estar vacío", nameof(name));

            if (price < 0)
                throw new ArgumentException("El precio no puede ser negativo", nameof(price));

            if (stock < 0)
                throw new ArgumentException("El stock no puede ser negativo", nameof(stock));

            Id = Guid.NewGuid();
            Name = name;
            Description = description ?? string.Empty;
            Price = price;
            Stock = stock;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(string name, string description, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del producto no puede estar vacío", nameof(name));

            if (price < 0)
                throw new ArgumentException("El precio no puede ser negativo", nameof(price));

            Name = name;
            Description = description ?? string.Empty;
            Price = price;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStock(int quantity)
        {
            if (Stock + quantity < 0)
                throw new InvalidOperationException("No hay suficiente stock disponible");

            Stock += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsAvailable() => Stock > 0;
    }
}
