using Savana.Order.API.Dtos;
using Savana.Order.API.Entities;
using Savana.Order.API.Extensions;
using Savana.Order.API.Interfaces;
using Savana.Order.API.Requests;
using Savana.Order.API.Requests.Params;
using Savana.Order.API.Specification;
using Treasures.Common.Helpers;
using Treasures.Common.Interfaces;

namespace Savana.Order.API.Services;

public class DeliveryService : IDeliveryService {
    private readonly IUnitOfWork _unitOfWork;
    public DeliveryService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<PagedList<DeliveryEntity>> GetDeliveries(DeliveryParams delParams) {
        var spec = new DeliverySpecification(delParams);
        return await _unitOfWork.Repository<DeliveryEntity>().GetPagedAsync(spec, delParams.Page, delParams.PageSize);
    }

    public async Task<DeliveryDto?> AddDeliveryMethod(DeliveryReq deliveryReq, string createdBy) {
        var newDelivery = new DeliveryEntity {
            Title = deliveryReq.Title, DeliveryTime = deliveryReq.DeliveryTime, Description = deliveryReq.Description,
            Price = deliveryReq.Price, CreatedBy = createdBy
        };

        var res = _unitOfWork.Repository<DeliveryEntity>().AddAsync(newDelivery);
        var result = await _unitOfWork.Complete();
        return result < 1 ? null : res.MapDeliveryToDto();
    }

    public async Task<DeliveryEntity?> GetDeliveryMethod(string methodId) => await _unitOfWork
        .Repository<DeliveryEntity>().GetByIdAsync(id: methodId);

    public async Task<DeliveryDto?> DeleteDeliveryMethod(DeliveryEntity existing, string updatedBy) {
        existing.Active = false;
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;
        return await SaveDeliveryMethodChanges(existing);
    }

    public async Task<DeliveryDto?> UpdateDeliveryMethod(DeliveryEntity existing, DeliveryReq req, string updatedBy) {
        existing.Title = req.Title ?? existing.Title;
        existing.Description = req.Description ?? existing.Description;
        existing.DeliveryTime = req.DeliveryTime ?? existing.DeliveryTime;
        existing.Price = req.Price;
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;
        return await SaveDeliveryMethodChanges(existing);
    }

    private async Task<DeliveryDto?> SaveDeliveryMethodChanges(DeliveryEntity existing) {
        var res = _unitOfWork.Repository<DeliveryEntity>().UpdateAsync(existing);
        var result = await _unitOfWork.Complete();
        return result < 1 ? null : res.MapDeliveryToDto();
    }
}