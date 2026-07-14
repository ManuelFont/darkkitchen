namespace DarkKitchen.Domain.Entities;

public sealed class OrderItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; }
    public int Quantity { get; private set; }
    public decimal Subtotal => Product.Price * Quantity;
    public decimal Total => Product.DiscountedPrice() * Quantity;

    public OrderItem(Product product, int quantity)
    {
        if(product == null)
        {
            throw new ArgumentException("Product cannot be null.");
        }

        if(quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        Id = Guid.NewGuid();
        ProductId = product.Id;
        Product = product;
        Quantity = quantity;
    }

    private OrderItem()
    {
        Product = null!;
    }
}
