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

        bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

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
