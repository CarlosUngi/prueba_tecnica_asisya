using Asisya.Application.DTOs;

namespace Asisya.Application.UseCases;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
}
