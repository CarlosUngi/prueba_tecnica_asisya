using Asisya.Domain.Entities;

namespace Asisya.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime Expiration) GenerateToken(User user);
}
