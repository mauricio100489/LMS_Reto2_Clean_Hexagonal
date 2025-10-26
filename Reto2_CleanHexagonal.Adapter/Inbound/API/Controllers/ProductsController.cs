using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Reto2_CleanHexagonal.Adapter.Inbound.API.DTOs;
using Reto2_CleanHexagonal.Domain.Ports;

namespace Reto2_CleanHexagonal.Adapter.Inbound.API.Controllers
{
    /// <summary>
    /// Adaptador de entrada (Inbound Adapter) - Controlador REST API
    /// Expone los endpoints HTTP y transforma las peticiones al formato del dominio
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los productos");
            var products = await _productService.GetAllProductsAsync();
            var productDtos = products.Select(MapToDto);
            return Ok(productDtos);
        }

        /// <summary>
        /// Obtiene productos disponibles (con stock)
        /// </summary>
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAvailable()
        {
            _logger.LogInformation("Obteniendo productos disponibles");
            var products = await _productService.GetAvailableProductsAsync();
            var productDtos = products.Select(MapToDto);
            return Ok(productDtos);
        }

        /// <summary>
        /// Obtiene un producto por ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            _logger.LogInformation("Obteniendo producto con ID: {ProductId}", id);

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Producto con ID {ProductId} no encontrado", id);
                return NotFound(new { message = $"Producto con ID {id} no encontrado" });
            }

            return Ok(MapToDto(product));
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
        {
            _logger.LogInformation("Creando nuevo producto: {ProductName}", dto.Name);

            try
            {
                var product = await _productService.CreateProductAsync(
                    dto.Name,
                    dto.Description,
                    dto.Price,
                    dto.Stock);

                var productDto = MapToDto(product);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, productDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear producto");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto dto)
        {
            _logger.LogInformation("Actualizando producto con ID: {ProductId}", id);

            try
            {
                var product = await _productService.UpdateProductAsync(id, dto.Name, dto.Description, dto.Price);
                return Ok(MapToDto(product));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Producto con ID {ProductId} no encontrado", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar producto");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el stock de un producto
        /// </summary>
        [HttpPatch("{id:guid}/stock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockDto dto)
        {
            _logger.LogInformation("Actualizando stock del producto {ProductId} en {Quantity}", id, dto.Quantity);

            try
            {
                await _productService.UpdateStockAsync(id, dto.Quantity);
                return Ok(new { message = "Stock actualizado correctamente" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Producto con ID {ProductId} no encontrado", id);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error al actualizar stock");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un producto
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Eliminando producto con ID: {ProductId}", id);

            try
            {
                await _productService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Producto con ID {ProductId} no encontrado", id);
                return NotFound(new { message = ex.Message });
            }
        }

        private static ProductDto MapToDto(Domain.Models.Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                IsAvailable = product.IsAvailable(),
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
