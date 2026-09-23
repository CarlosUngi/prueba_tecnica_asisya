using System.Data;
using Asisya.Domain.Entities;
using Asisya.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Asisya.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.ProductId == id, cancellationToken);
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm, 
        int? categoryId, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(p => p.ProductName.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.ProductId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BulkInsertAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default)
    {
        // Estrategia de Inserción Masiva Ultra-Rápida con PostgreSQL NpgsqlBinaryImporter (COPY BINARY)
        var dbConnection = _context.Database.GetDbConnection();
        if (dbConnection is NpgsqlConnection npgsqlConnection)
        {
            if (npgsqlConnection.State != ConnectionState.Open)
            {
                await npgsqlConnection.OpenAsync(cancellationToken);
            }

            const string copyCommand = @"
                COPY products (
                    product_name, 
                    supplier_id, 
                    category_id, 
                    quantity_per_unit, 
                    unit_price, 
                    units_in_stock, 
                    units_on_order, 
                    reorder_level, 
                    discontinued, 
                    created_at
                ) FROM STDIN (FORMAT BINARY)";

            await using var writer = await npgsqlConnection.BeginBinaryImportAsync(copyCommand, cancellationToken);

            foreach (var product in products)
            {
                await writer.StartRowAsync(cancellationToken);
                await writer.WriteAsync(product.ProductName, NpgsqlDbType.Varchar, cancellationToken);
                await writer.WriteAsync(product.SupplierId, NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(product.CategoryId, NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(product.QuantityPerUnit, NpgsqlDbType.Varchar, cancellationToken);
                await writer.WriteAsync(product.UnitPrice, NpgsqlDbType.Numeric, cancellationToken);
                await writer.WriteAsync(product.UnitsInStock, NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(product.UnitsOnOrder, NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(product.ReorderLevel, NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(product.Discontinued, NpgsqlDbType.Boolean, cancellationToken);
                await writer.WriteAsync(product.CreatedAt, NpgsqlDbType.TimestampTz, cancellationToken);
            }

            await writer.CompleteAsync(cancellationToken);
        }
        else
        {
            // Estrategia Fallback con EF Core Batching si la conexión no es Npgsql directa
            const int chunkSize = 5000;
            var productList = products.ToList();

            for (int i = 0; i < productList.Count; i += chunkSize)
            {
                var chunk = productList.Skip(i).Take(chunkSize);
                await _context.Products.AddRangeAsync(chunk, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                _context.ChangeTracker.Clear();
            }
        }
    }
}
