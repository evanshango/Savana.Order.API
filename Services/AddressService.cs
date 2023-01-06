using Savana.Order.API.Dtos;
using Savana.Order.API.Entities;
using Savana.Order.API.Extensions;
using Savana.Order.API.Interfaces;
using Savana.Order.API.Requests;
using Savana.Order.API.Requests.Params;
using Savana.Order.API.Specification;
using Treasures.Common.Interfaces;

namespace Savana.Order.API.Services;

public class AddressService : IAddressService {
    private readonly IUnitOfWork _unitOfWork;
    public AddressService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<AddressDto?> GetUserAddress(AddressParams addressParams) {
        var addressSpec = new AddressSpecification(addressParams);
        var existing = await _unitOfWork.Repository<AddressEntity>().GetEntityWithSpec(addressSpec);
        return existing?.MapAddressToDto();
    }

    public async Task<AddressDto?> AddAddress(AddressReq addressReq, string createdBy) {
        var existing = await FetchUserAddress(createdBy);
        if (existing != null) {
            existing.Name = addressReq.Name ?? existing.Name;
            existing.Email = addressReq.Email ?? existing.Email;
            existing.Phone = addressReq.Phone ?? existing.Phone;
            existing.Street = addressReq.Street ?? existing.Street;
            existing.Building = addressReq.Building ?? existing.Building;
            existing.ZipCode = addressReq.ZipCode ?? existing.ZipCode;
            existing.City = addressReq.City ?? existing.City;
            existing.Country = addressReq.Country ?? existing.Country;
            existing.UpdatedBy = createdBy;
            existing.UpdatedAt = DateTime.UtcNow;

            return await SaveAddressChanges(existing);
        }

        var newAddress = new AddressEntity {
            Name = addressReq.Name, Email = addressReq.Email, Phone = addressReq.Phone, Street = addressReq.Street,
            Building = addressReq.Building, ZipCode = addressReq.ZipCode, City = addressReq.City,
            Country = addressReq.Country, CreatedBy = createdBy
        };

        var res = _unitOfWork.Repository<AddressEntity>().AddAsync(newAddress);
        var result = await _unitOfWork.Complete();
        return result < 1 ? null : res.MapAddressToDto();
    }

    public async Task<AddressEntity?> GetAddress(string createdBy) => await FetchUserAddress(createdBy);

    public async Task<AddressEntity?> GetAddressById(string addressId) => await _unitOfWork
        .Repository<AddressEntity>().GetByIdAsync(addressId);

    public async Task<AddressDto?> DeleteAddress(AddressEntity existingAddress, string updatedBy) {
        existingAddress.UpdatedBy = updatedBy;
        existingAddress.UpdatedAt = DateTime.UtcNow;
        existingAddress.Active = false;
        return await SaveAddressChanges(existingAddress);
    }

    private async Task<AddressEntity?> FetchUserAddress(string email) {
        var addressSpec = new AddressSpecification(email);
        return await _unitOfWork.Repository<AddressEntity>().GetEntityWithSpec(addressSpec);
    }

    private async Task<AddressDto?> SaveAddressChanges(AddressEntity existing) {
        var res = _unitOfWork.Repository<AddressEntity>().UpdateAsync(existing);
        var result = await _unitOfWork.Complete();
        return result < 1 ? null : res.MapAddressToDto();
    }
}