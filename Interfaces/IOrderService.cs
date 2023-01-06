using Savana.Order.API.Dtos;
using Savana.Order.API.Entities;
using Savana.Order.API.Requests;
using Savana.Order.API.Requests.Params;
using Treasures.Common.Helpers;

namespace Savana.Order.API.Interfaces;

public interface IOrderService {
    Task<OrderDto?> CreateOrder(
        OrderReq orderReq, VoucherEntity? voucher, DeliveryEntity delivery, AddressEntity address, string createdBy
    );
    Task<PagedList<OrderEntity>> GetOrders(OrderParams orderParams);
    Task<OrderDto?> GetOrder(string orderId);
}