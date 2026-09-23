using Asisya.Domain.Entities;

namespace Asisya.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm, 
        int? categoryId, 
        CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task BulkInsertAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
}
