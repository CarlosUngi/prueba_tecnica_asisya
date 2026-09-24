using Asisya.Application.DTOs;
using Asisya.Application.Interfaces;
using Asisya.Application.UseCases;
using Asisya.Domain.Entities;
using Asisya.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Backend.UnitTests;

public class AuthServiceTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IJwtTokenGenerator _jwtTokenGeneratorMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _jwtTokenGeneratorMock = Substitute.For<IJwtTokenGenerator>();
        _sut = new AuthService(_userRepositoryMock, _jwtTokenGeneratorMock);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponseDtoWithToken()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "admin", Password = "Admin123!" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@asisya.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "Admin"
        };

        _userRepositoryMock.GetByUsernameAsync(loginDto.Username, Arg.Any<CancellationToken>())
            .Returns(user);

        _jwtTokenGeneratorMock.GenerateToken(user)
            .Returns(("mock.jwt.token", DateTime.UtcNow.AddHours(2)));

        // Act
        var result = await _sut.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("mock.jwt.token");
        result.Username.Should().Be("admin");
        result.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "admin", Password = "WrongPassword" };
        var user = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
        };

        _userRepositoryMock.GetByUsernameAsync(loginDto.Username, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var act = async () => await _sut.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*incorrectos*");
    }
}
