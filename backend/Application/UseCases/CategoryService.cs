using Asisya.Application.DTOs;
using Asisya.Application.Mappings;
using Asisya.Domain.Repositories;

namespace Asisya.Application.UseCases;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        return category?.ToDto();
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);

        return categories.Select(c => c.ToDto());
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _categoryRepository.GetByNameAsync(dto.CategoryName, cancellationToken);
        if (existing != null)
        {
            throw new InvalidOperationException($"La categoría '{dto.CategoryName}' ya existe.");
        }

        var category = dto.ToEntity();
        var created = await _categoryRepository.AddAsync(category, cancellationToken);
        return created.ToDto();
    }
}
