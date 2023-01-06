using System.ComponentModel.DataAnnotations;

namespace Savana.Order.API.Requests;

public class OrderReq {
    [Required(ErrorMessage = "BuyerId is required")]
    public string? BuyerId { get; set; }

    [Required(ErrorMessage = "Delivery Method is required")]
    public string? DeliveryId { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public string? AddressId { get; set; }

    public string? VoucherCode { get; set; }

    [Required(ErrorMessage = "Payment option is required")]
    public string? PaymentOption { get; set; }

    public List<BasketItem> Items { get; set; } = new();
}

public class BasketItem {
    public string? ProductId { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
}