using System.ComponentModel.DataAnnotations;
using Treasures.Common.Helpers;

namespace Savana.Order.API.Entities;

public class DeliveryEntity : BaseEntity {
    [Required] public string? Title { get; set; }
    [Required] public string? DeliveryTime { get; set; }
    public string? Description { get; set; }
    [Required] public double Price { get; set; }
}