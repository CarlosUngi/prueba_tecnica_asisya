using Asisya.Application.DTOs;
using Asisya.Application.UseCases;
using Asisya.Domain.Entities;
using Asisya.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Backend.UnitTests;

public class ProductServiceTests
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _sut = new ProductService(_productRepositoryMock, _categoryRepositoryMock);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProductDto()
    {
        // Arrange
        var productId = 1;
        var product = new Product
        {
            ProductId = productId,
            ProductName = "Servidor PowerEdge R750",
            CategoryId = 1,
            Category = new Category { CategoryId = 1, CategoryName = "SERVIDORES" },
            UnitPrice = 2500m,
            UnitsInStock = 10
        };

        _productRepositoryMock.GetByIdWithCategoryAsync(productId, Arg.Any<CancellationToken>())
            .Returns(product);

        // Act
        var result = await _sut.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.ProductId.Should().Be(productId);
        result.ProductName.Should().Be("Servidor PowerEdge R750");
        result.CategoryName.Should().Be("SERVIDORES");
    }

    [Fact]
    public async Task CreateAsync_WhenCategoryDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            ProductName = "Test Product",
            CategoryId = 999
        };

        _categoryRepositoryMock.GetByIdAsync(dto.CategoryId, Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        // Act
        var act = async () => await _sut.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*no existe*");
    }

    [Fact]
    public async Task GenerateBulkProductsAsync_GeneratesRequestedCountAndCallsBulkInsert()
    {
        // Arrange
        var count = 500;
        var servidores = new Category { CategoryId = 1, CategoryName = "SERVIDORES" };
        var cloud = new Category { CategoryId = 2, CategoryName = "CLOUD" };

        _categoryRepositoryMock.GetByNameAsync("SERVIDORES", Arg.Any<CancellationToken>())
            .Returns(servidores);
        _categoryRepositoryMock.GetByNameAsync("CLOUD", Arg.Any<CancellationToken>())
            .Returns(cloud);

        // Act
        var result = await _sut.GenerateBulkProductsAsync(count);

        // Assert
        result.Should().Be(count);
        await _productRepositoryMock.Received(1).BulkInsertAsync(
            Arg.Is<IEnumerable<Product>>(list => list.Count() == count),
            Arg.Any<CancellationToken>());
    }
}
