using Asisya.Application.DTOs;
using Asisya.Domain.Entities;

namespace Asisya.Application.Mappings;

public static class CategoryMappingExtensions
{
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            Picture = category.Picture
        };
    }

    public static Category ToEntity(this CreateCategoryDto dto)
    {
        return new Category
        {
            CategoryName = dto.CategoryName,
            Description = dto.Description,
            Picture = dto.Picture
        };
    }
}
