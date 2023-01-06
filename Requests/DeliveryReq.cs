using System.ComponentModel.DataAnnotations;

namespace Savana.Order.API.Requests;

public class DeliveryReq {
    [Required(ErrorMessage = "Delivery Title is required")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Delivery Timeline is required")]
    public string? DeliveryTime { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "Delivery Price is required")]
    public double Price { get; set; }
}