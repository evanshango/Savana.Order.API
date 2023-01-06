using System.ComponentModel.DataAnnotations;

namespace Savana.Order.API.Entities;

public class ProductEntity {
    [Key] [Required] public string? ProductId { get; set; }
    public string? Name { get; set; }
    public double InitialPrice { get; set; }
    public double FinalPrice { get; set; }
    public string? ImageUrl { get; set; }
    public string? Brand { get; set; }
    public int Stock { get; set; }
    public DateTime? PromoExpiry { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool Active { get; set; } = true;
    public string? Owner { get; set; }

    public double GetFinalPrice() => DateTime.UtcNow > PromoExpiry ? InitialPrice : FinalPrice;
}