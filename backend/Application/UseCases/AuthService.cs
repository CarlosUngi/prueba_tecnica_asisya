using Asisya.Application.DTOs;
using Asisya.Application.Interfaces;
using Asisya.Domain.Repositories;

namespace Asisya.Application.UseCases;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUserRepository userRepository, 
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(loginDto.Username, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");
        }

        bool isValidPassword = false;

        // 1. Verificación por texto plano (si en la BD está como 'Admin123!')
        if (user.PasswordHash == loginDto.Password)
        {
            isValidPassword = true;
        }
        // 2. Verificación por BCrypt Hash (si empieza por $2a$, $2b$ o $2y$)
        else if (user.PasswordHash.StartsWith("$2a$") || 
                 user.PasswordHash.StartsWith("$2b$") || 
                 user.PasswordHash.StartsWith("$2y$"))
        {
            try
            {
                isValidPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            }
            catch
            {
                // Si la sal o el hash en la BD está corrupto o mal formado, no es una contraseña válida
                isValidPassword = false;
            }
        }

        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");
        }

        var (token, expiration) = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            Expiration = expiration
        };
    }
}
