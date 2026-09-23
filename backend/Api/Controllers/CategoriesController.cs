using Asisya.Application.DTOs;
using Asisya.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Asisya.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Lista todas las categorías registradas.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Listar categorías", Description = "Obtiene el catálogo completo de categorías registradas en la base de datos.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Lista de categorías obtenida con éxito", typeof(IEnumerable<CategoryDto>))]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene el detalle de una categoría por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Obtener categoría por ID", Description = "Busca los datos de una categoría específica por su identificador único.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Categoría encontrada", typeof(CategoryDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Categoría no encontrada")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category == null) return NotFound(new { message = $"Categoría con ID {id} no encontrada." });
        return Ok(category);
    }

    /// <summary>
    /// Crea una nueva categoría en el sistema.
    /// </summary>
    [HttpPost]
    [Authorize]
    [SwaggerOperation(Summary = "Crear categoría", Description = "Permite registrar una nueva categoría. Requiere token JWT.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Categoría creada con éxito", typeof(CategoryDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Datos inválidos o categoría duplicada")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "No autorizado")]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryDto dto, 
        CancellationToken cancellationToken)
    {
        var created = await _categoryService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.CategoryId }, created);
    }
}
