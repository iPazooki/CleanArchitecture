namespace CleanArchitecture.Domain.Entities.Order;

public sealed class OrderItem : Entity
{
    public int OrderId { get; set; }

    public int BookId { get; init; } 
    
    public required Book.Book Book { get; set; }
    
    public int Quantity { get; set; } 

    public decimal UnitPrice { get; set; }
    
    public byte[]? RowVersion { get; set; }
}