using Savana.Order.API.Dtos;
using Savana.Order.API.Entities;

namespace Savana.Order.API.Extensions;

public static class DeliveryExtensions {
    public static DeliveryDto MapDeliveryToDto(this DeliveryEntity? delivery) => new() {
        Id = delivery?.Id, Title = delivery?.Title, Description = delivery?.Description, Price = delivery!.Price,
        DeliveryTimeLine = delivery.DeliveryTime, CreatedAt = delivery.CreatedAt
    };
}