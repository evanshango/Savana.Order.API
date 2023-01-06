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
    private readonly ILogger<AddressService> _logger;

    public AddressService(IUnitOfWork unitOfWork, ILogger<AddressService> logger) {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

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

        if (result >= 1) {
            _logger.LogInformation("Address for user with email {Email} created", res.CreatedBy);
            return res.MapAddressToDto();
        }

        _logger.LogError("Unable to create address for user with email {Email}", addressReq.Email);
        return null;
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
        var address = await _unitOfWork.Repository<AddressEntity>().GetEntityWithSpec(addressSpec);

        if (address != null) return address;
        _logger.LogWarning("Address for user with email {Email} not found", email);
        return null;
    }

    private async Task<AddressDto?> SaveAddressChanges(AddressEntity existing) {
        var res = _unitOfWork.Repository<AddressEntity>().UpdateAsync(existing);
        var result = await _unitOfWork.Complete();

        if (result >= 1) {
            _logger.LogInformation("Address for user with email {Email} updated", res.CreatedBy);
            return res.MapAddressToDto();
        }

        _logger.LogError("Unable to update address for user with email {Email}", existing.CreatedBy);
        return null;
    }
}