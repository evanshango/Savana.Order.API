using Savana.Order.API.Dtos;
using Savana.Order.API.Entities;
using Savana.Order.API.Requests;
using Savana.Order.API.Requests.Params;
using Treasures.Common.Helpers;

namespace Savana.Order.API.Interfaces;

public interface IDeliveryService {
    Task<PagedList<DeliveryEntity>> GetDeliveries(DeliveryParams delParams);
    Task<DeliveryDto?> AddDeliveryMethod(DeliveryReq deliveryReq, string createdBy);
    Task<DeliveryEntity?> GetDeliveryMethod(string methodId);
    Task<DeliveryDto?> DeleteDeliveryMethod(DeliveryEntity existing, string updatedBy);
    Task<DeliveryDto?> UpdateDeliveryMethod(DeliveryEntity existing, DeliveryReq deliveryReq, string updatedBy);
}