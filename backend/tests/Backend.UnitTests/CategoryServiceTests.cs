using Asisya.Application.DTOs;
using Asisya.Application.UseCases;
using Asisya.Domain.Entities;
using Asisya.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Backend.UnitTests;

public class CategoryServiceTests
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _sut = new CategoryService(_categoryRepositoryMock);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsCategoryDto()
    {
        // Arrange
        var categoryId = 1;
        var category = new Category
        {
            CategoryId = categoryId,
            CategoryName = "SERVIDORES",
            Description = "Test Category"
        };

        _categoryRepositoryMock.GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(category);

        // Act
        var result = await _sut.GetByIdAsync(categoryId);

        // Assert
        result.Should().NotBeNull();
        result!.CategoryId.Should().Be(categoryId);
        result.CategoryName.Should().Be("SERVIDORES");
    }

    [Fact]
    public async Task CreateAsync_WhenNameIsUnique_CreatesCategorySuccessfully()
    {
        // Arrange
        var dto = new CreateCategoryDto
        {
            CategoryName = "STORAGE",
            Description = "Sistemas de Almacenamiento"
        };

        _categoryRepositoryMock.GetByNameAsync(dto.CategoryName, Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        _categoryRepositoryMock.AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var cat = callInfo.Arg<Category>();
                cat.CategoryId = 10;
                return cat;
            });

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.CategoryId.Should().Be(10);
        result.CategoryName.Should().Be("STORAGE");
        await _categoryRepositoryMock.Received(1).AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_WhenCategoryAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new CreateCategoryDto { CategoryName = "SERVIDORES" };
        var existingCategory = new Category { CategoryId = 1, CategoryName = "SERVIDORES" };

        _categoryRepositoryMock.GetByNameAsync(dto.CategoryName, Arg.Any<CancellationToken>())
            .Returns(existingCategory);

        // Act
        var act = async () => await _sut.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ya existe*");
    }
}
