using Savana.Order.API.Dtos;
using Savana.Order.API.Entities;
using Savana.Order.API.Requests;
using Savana.Order.API.Requests.Params;

namespace Savana.Order.API.Interfaces;

public interface IAddressService {
    Task<AddressDto?> GetUserAddress(AddressParams addressParams);
    Task<AddressDto?> AddAddress(AddressReq addressReq, string createdBy);
    Task<AddressEntity?> GetAddress(string createdBy);
    Task<AddressEntity?> GetAddressById(string addressId);
    Task<AddressDto?> DeleteAddress(AddressEntity existingAddress, string updatedBy);
}