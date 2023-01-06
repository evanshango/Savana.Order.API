using System.ComponentModel.DataAnnotations;
using Treasures.Common.Helpers;

namespace Savana.Order.API.Entities;

public class OrderEntity : BaseEntity {
    [Required] public string? BuyerId { get; set; }
    [Required] public string? BuyerEmail { get; set; }
    [Required] public string? AddressId { get; set; }
    public virtual AddressEntity? Address { get; set; }
    public List<OrderItem>? OrderItems { get; set; } = new();
    [Required] public double SubTotal { get; set; }
    [Required] public double Totals { get; set; }
    [Required] public double TotalCost { get; set; }
    [Required] public string? DeliveryId { get; set; }
    public virtual DeliveryEntity? DeliveryMethod { get; set; }
    [Required] public string? OrderStatus { get; set; } = "OrderPending";
    [Required] public string? PaymentStatus { get; set; } = "PaymentPending";
    public string? PaymentReference { get; set; }
    public string? ResponseMessage { get; set; }
    [Required] public string? PaymentOption { get; set; }

    public double GetTotalCost() => SubTotal + DeliveryMethod!.Price;
}