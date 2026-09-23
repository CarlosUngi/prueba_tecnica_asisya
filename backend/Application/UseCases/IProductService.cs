using Asisya.Application.DTOs;

namespace Asisya.Application.UseCases;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResultDto<ProductDto>> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm, 
        int? categoryId, 
        CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GenerateBulkProductsAsync(int count = 100000, CancellationToken cancellationToken = default);
}
