namespace Savana.Order.API.Dtos; 

public class OrderItemDto {
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? Brand { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
}