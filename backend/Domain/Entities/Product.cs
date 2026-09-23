namespace Asisya.Domain.Entities;

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string? QuantityPerUnit { get; set; }
    public decimal UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
    public int UnitsOnOrder { get; set; }
    public int ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
