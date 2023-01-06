namespace Savana.Order.API.Dtos; 

public class OrderDto {
    public string? Id { get; set; }
    public double SubTotal { get; set; }
    public double TotalCost { get; set; }
    public string? OrderStatus { get; set; }
    public string? PaymentOption { get; set; }  
    public string? PaymentStatus { get; set; }
    public AddressDto? DeliveryAddress { get; set; }
    public DeliveryDto? DeliveryMethod { get; set; }
    public List<OrderItemDto>? Items { get; set; }
    public DateTime CreatedAt { get; set; }
}