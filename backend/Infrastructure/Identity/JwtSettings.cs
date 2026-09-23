namespace Asisya.Infrastructure.Identity;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";
    public string Secret { get; set; } = "AsisyaPruebaTecnicaSuperSecretKey2026Net8JwtValidation!";
    public string Issuer { get; set; } = "AsisyaApi";
    public string Audience { get; set; } = "AsisyaClients";
    public int ExpiryMinutes { get; set; } = 120;
}
