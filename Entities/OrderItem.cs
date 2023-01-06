using System.ComponentModel.DataAnnotations;

namespace Savana.Order.API.Entities;

public class OrderItem {
    [Key] [Required] public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? ProductId { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? Brand { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string? Owner { get; set; }
    public DateTime CreatedAt { get; set; }
}