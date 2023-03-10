namespace Savana.Order.API.Dtos; 

public class DeliveryDto {
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? DeliveryTimeLine { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public DateTime CreatedAt { get; set; }
}