using Asisya.Application.DTOs;
using Asisya.Domain.Entities;

namespace Asisya.Application.Mappings;

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            SupplierId = product.SupplierId,
            SupplierName = product.Supplier?.CompanyName,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.CategoryName ?? string.Empty,
            CategoryPicture = product.Category?.Picture,
            QuantityPerUnit = product.QuantityPerUnit,
            UnitPrice = product.UnitPrice,
            UnitsInStock = product.UnitsInStock,
            UnitsOnOrder = product.UnitsOnOrder,
            ReorderLevel = product.ReorderLevel,
            Discontinued = product.Discontinued,
            CreatedAt = product.CreatedAt
        };
    }

    public static Product ToEntity(this CreateProductDto dto)
    {
        return new Product
        {
            ProductName = dto.ProductName,
            SupplierId = dto.SupplierId,
            CategoryId = dto.CategoryId,
            QuantityPerUnit = dto.QuantityPerUnit,
            UnitPrice = dto.UnitPrice,
            UnitsInStock = dto.UnitsInStock,
            UnitsOnOrder = dto.UnitsOnOrder,
            ReorderLevel = dto.ReorderLevel,
            Discontinued = dto.Discontinued
        };
    }

    public static void UpdateEntity(this Product product, UpdateProductDto dto)
    {
        product.ProductName = dto.ProductName;
        product.SupplierId = dto.SupplierId;
        product.CategoryId = dto.CategoryId;
        product.QuantityPerUnit = dto.QuantityPerUnit;
        product.UnitPrice = dto.UnitPrice;
        product.UnitsInStock = dto.UnitsInStock;
        product.UnitsOnOrder = dto.UnitsOnOrder;
        product.ReorderLevel = dto.ReorderLevel;
        product.Discontinued = dto.Discontinued;
    }
}
