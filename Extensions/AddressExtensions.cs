using Savana.Order.API.Dtos;
using Savana.Order.API.Entities;

namespace Savana.Order.API.Extensions;

public static class AddressExtensions {
    public static AddressDto MapAddressToDto(this AddressEntity? address) => new() {
        Id = address?.Id, Name = address?.Name, Email = address?.Email, Phone = address?.Phone,
        Building = address?.Building, City = address?.City, Country = address?.Country, Street = address?.Street,
        ZipCode = address?.ZipCode, CreatedAt = address!.CreatedAt
    };
}