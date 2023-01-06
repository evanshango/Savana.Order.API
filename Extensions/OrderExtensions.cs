using Savana.Order.API.Dtos;
using Savana.Order.API.Entities;

namespace Savana.Order.API.Extensions;

public static class OrderExtensions {
    public static OrderDto MapOrderToDto(this OrderEntity order) => new() {
        Id = order.Id, SubTotal = order.SubTotal, TotalCost = order.TotalCost, CreatedAt = order.CreatedAt,
        OrderStatus = order.OrderStatus, PaymentOption = order.PaymentOption, PaymentStatus = order.PaymentStatus,
        DeliveryAddress = order.Address.MapAddressToDto(), DeliveryMethod = order.DeliveryMethod.MapDeliveryToDto(),
        Items = order.OrderItems?.Select(i => i.MapOrderItemToDto()).ToList()
    };

    private static OrderItemDto MapOrderItemToDto(this OrderItem item) => new() {
        Id = item.ProductId, Name = item.Name, Brand = item.Brand, Price = item.Price, Quantity = item.Quantity,
        ImageUrl = item.ImageUrl
    };
}