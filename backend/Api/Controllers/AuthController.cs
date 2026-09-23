using Asisya.Application.DTOs;
using Asisya.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Asisya.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Inicia sesión y obtiene un Token JWT de autenticación.
    /// </summary>
    /// <param name="loginDto">Credenciales de acceso (Usuario y Contraseña)</param>
    /// <returns>Token JWT Bearer e información del usuario</returns>
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Autenticación de usuario", Description = "Permite a los usuarios autenticarse y recibir un token JWT para acceder a los endpoints protegidos.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Autenticación exitosa", typeof(AuthResponseDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Credenciales incorrectas")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginDto loginDto, 
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(loginDto, cancellationToken);
        return Ok(result);
    }
}
