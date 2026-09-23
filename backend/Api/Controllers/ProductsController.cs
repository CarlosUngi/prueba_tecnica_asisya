using Asisya.Application.DTOs;
using Asisya.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Asisya.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Lista productos con paginación, filtros por categoría y búsqueda textual.
    /// </summary>
    /// <param name="pageNumber">Número de página (por defecto 1)</param>
    /// <param name="pageSize">Cantidad de registros por página (por defecto 10, máx 100)</param>
    /// <param name="searchTerm">Término de búsqueda opcional por nombre del producto</param>
    /// <param name="categoryId">Filtro opcional por ID de categoría</param>
    [HttpGet]
    [SwaggerOperation(Summary = "Listar productos paginados", Description = "Obtiene el listado de productos de forma paginada con soporte de filtros y búsquedas.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Productos obtenidos exitosamente", typeof(PagedResultDto<ProductDto>))]
    public async Task<ActionResult<PagedResultDto<ProductDto>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetPagedAsync(pageNumber, pageSize, searchTerm, categoryId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene el detalle de un producto específico por su ID, incluyendo la foto de su categoría.
    /// </summary>
    /// <param name="id">ID del producto</param>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Obtener producto por ID con foto de categoría", Description = "Retorna el detalle completo de un producto incluyendo la URL de la imagen de su categoría.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Producto encontrado", typeof(ProductDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Producto no encontrado")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null) return NotFound(new { message = $"Producto con ID {id} no encontrado." });
        return Ok(product);
    }

    /// <summary>
    /// Crea un nuevo producto de forma individual.
    /// </summary>
    [HttpPost]
    [Authorize]
    [SwaggerOperation(Summary = "Crear un producto", Description = "Registra un producto individual en la base de datos. Requiere JWT.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Producto creado exitosamente", typeof(ProductDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Datos inválidos o categoría inexistente")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "No autorizado")]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateProductDto dto, 
        CancellationToken cancellationToken)
    {
        var created = await _productService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
    }

    /// <summary>
    /// Genera e inserta masivamente 100,000 productos aleatorios de forma ultra-veloz.
    /// </summary>
    /// <param name="count">Cantidad de productos a generar (Por defecto 100,000)</param>
    [HttpPost("bulk")]
    [Authorize]
    [SwaggerOperation(Summary = "Carga masiva de 100,000 productos", Description = "Genera e inserta en tiempo récord 100,000 productos aleatorios asociados a las categorías SERVIDORES y CLOUD mediante PostgreSQL Binary COPY.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Carga masiva completada exitosamente")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "No autorizado")]
    public async Task<IActionResult> GenerateBulk([FromQuery] int count = 100000, CancellationToken cancellationToken = default)
    {
        var insertedCount = await _productService.GenerateBulkProductsAsync(count, cancellationToken);
        return Ok(new
        {
            message = $"Se insertaron exitosamente {insertedCount:N0} productos en la base de datos.",
            count = insertedCount,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Actualiza un producto existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [SwaggerOperation(Summary = "Actualizar producto", Description = "Actualiza los campos de un producto por su ID. Requiere JWT.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Producto actualizado correctamente")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Producto no encontrado")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "No autorizado")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto, CancellationToken cancellationToken)
    {
        var success = await _productService.UpdateAsync(id, dto, cancellationToken);
        if (!success) return NotFound(new { message = $"Producto con ID {id} no encontrado." });
        return Ok(new { message = "Producto actualizado correctamente." });
    }

    /// <summary>
    /// Elimina un producto por su ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    [SwaggerOperation(Summary = "Eliminar producto", Description = "Elimina un producto físicamente de la base de datos. Requiere JWT.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Producto eliminado correctamente")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Producto no encontrado")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "No autorizado")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var success = await _productService.DeleteAsync(id, cancellationToken);
        if (!success) return NotFound(new { message = $"Producto con ID {id} no encontrado." });
        return Ok(new { message = "Producto eliminado correctamente." });
    }
}
