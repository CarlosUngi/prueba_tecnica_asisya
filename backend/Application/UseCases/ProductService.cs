using Asisya.Application.DTOs;
using Asisya.Application.Mappings;
using Asisya.Domain.Entities;
using Asisya.Domain.Repositories;

namespace Asisya.Application.UseCases;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(
        IProductRepository productRepository, 
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id, cancellationToken);
        return product?.ToDto();
    }

    public async Task<PagedResultDto<ProductDto>> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm, 
        int? categoryId, 
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var (items, totalCount) = await _productRepository.GetPagedAsync(
            pageNumber, pageSize, searchTerm, categoryId, cancellationToken);

        return new PagedResultDto<ProductDto>
        {
            Items = items.Select(p => p.ToDto()),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new KeyNotFoundException($"La categoría con ID {dto.CategoryId} no existe.");
        }

        var product = dto.ToEntity();
        var created = await _productRepository.AddAsync(product, cancellationToken);
        return created.ToDto();
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id, cancellationToken);
        if (product == null) return false;

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new KeyNotFoundException($"La categoría con ID {dto.CategoryId} no existe.");
        }

        product.UpdateEntity(dto);
        await _productRepository.UpdateAsync(product, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id, cancellationToken);
        if (product == null) return false;

        await _productRepository.DeleteAsync(product, cancellationToken);
        return true;
    }

    public async Task<int> GenerateBulkProductsAsync(int count = 100000, CancellationToken cancellationToken = default)
    {
        // Buscar o crear categorías 'SERVIDORES' y 'CLOUD'
        var servidoresCategory = await _categoryRepository.GetByNameAsync("SERVIDORES", cancellationToken);
        if (servidoresCategory == null)
        {
            servidoresCategory = await _categoryRepository.AddAsync(new Category
            {
                CategoryName = "SERVIDORES",
                Description = "Servidores físicos y dedicados de alto rendimiento",
                Picture = "https://via.placeholder.com/150?text=Servidores"
            }, cancellationToken);
        }

        var cloudCategory = await _categoryRepository.GetByNameAsync("CLOUD", cancellationToken);
        if (cloudCategory == null)
        {
            cloudCategory = await _categoryRepository.AddAsync(new Category
            {
                CategoryName = "CLOUD",
                Description = "Instancias y servicios en la nube computacional",
                Picture = "https://via.placeholder.com/150?text=Cloud"
            }, cancellationToken);
        }

        var categoryIds = new[] { servidoresCategory.CategoryId, cloudCategory.CategoryId };
        var random = new Random();
        var prefixes = new[] { "Dell PowerEdge", "HP ProLiant", "AWS EC2 Instance", "Azure Virtual Machine", "Cisco UCS", "Lenovo ThinkSystem", "GCP Compute Engine" };
        var suffixes = new[] { "vCPU 8GB", "vCPU 16GB", "Dedicated 64GB", "Enterprise NVMe", "Cluster Node", "High Mem 128GB", "GPU Accelerator" };

        var productsToInsert = new List<Product>(count);

        for (int i = 1; i <= count; i++)
        {
            var prefix = prefixes[random.Next(prefixes.Length)];
            var suffix = suffixes[random.Next(suffixes.Length)];
            var categoryId = categoryIds[random.Next(categoryIds.Length)];

            productsToInsert.Add(new Product
            {
                ProductName = $"{prefix} {suffix} #{i}",
                CategoryId = categoryId,
                QuantityPerUnit = $"{random.Next(1, 4)} unidades",
                UnitPrice = Math.Round((decimal)(random.NextDouble() * 5000 + 50), 2),
                UnitsInStock = random.Next(1, 500),
                UnitsOnOrder = random.Next(0, 50),
                ReorderLevel = 10,
                Discontinued = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Ejecutar inserción masiva optimizada
        await _productRepository.BulkInsertAsync(productsToInsert, cancellationToken);
        return count;
    }
}
