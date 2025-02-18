namespace CleanArchitecture.Domain.Errors;

public static class OrderErrors
{
    public static readonly Error OrderStatusInvalid = new("The order status is invalid.", "OrderStatusInvalid");

    public static readonly Error OrderItemQuantityInvalid = new("The order item quantity is invalid.", "OrderItemQuantityInvalid");
    
    public static readonly Error OrderItemUnitPriceInvalid = new("The order item unit price is invalid.", "OrderItemUnitPriceInvalid");
}