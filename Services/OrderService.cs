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

public class OrderService : IOrderService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductService _productService;
    private readonly IVoucherService _voucherService;

    public OrderService(IUnitOfWork unitOfWork, IProductService productService, IVoucherService voucherService) {
        _unitOfWork = unitOfWork;
        _productService = productService;
        _voucherService = voucherService;
    }

    public async Task<OrderDto?> CreateOrder(
        OrderReq orderReq, VoucherEntity? voucher, DeliveryEntity delivery, AddressEntity address, string createdBy
    ) {
        var orderItems = new List<OrderItem>();
        foreach (var item in orderReq.Items) {
            var existingProd = await _productService.GetProductById(item.ProductId!);
            if (existingProd == null) continue;
            if (existingProd.Stock > item.Quantity) {
                var price = existingProd.GetFinalPrice();
                if (DateTime.UtcNow > existingProd.PromoExpiry) {
                    existingProd.FinalPrice = existingProd.InitialPrice;
                    await _productService.UpdateProductPromoExpiry(existingProd, "promo");
                }

                var orderItem = new OrderItem {
                    ProductId = item.ProductId, Name = item.Name, ImageUrl = item.ImageUrl, Price = price,
                    Brand = existingProd.Brand, Owner = existingProd.Owner, Quantity = item.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                orderItems.Add(orderItem);
                existingProd.Stock -= orderItem.Quantity;
            } else {
                existingProd.Stock = existingProd.Stock;
            }

            await _productService.UpdateProductQuantity(existingProd, "stock");
        }

        var totals = orderItems.Sum(o => o.Price * o.Quantity);
        var subTotal = voucher != null ? totals * (voucher.Discount / 100) : totals;

        var newOrder = new OrderEntity {
            BuyerId = orderReq.BuyerId, BuyerEmail = createdBy, OrderItems = orderItems, Totals = totals,
            SubTotal = subTotal, CreatedBy = createdBy, AddressId = address.Id, Address = address,
            DeliveryId = delivery.Id, DeliveryMethod = delivery, PaymentOption = orderReq.PaymentOption
        };
        newOrder.TotalCost = newOrder.GetTotalCost();

        var res = _unitOfWork.Repository<OrderEntity>().AddAsync(newOrder);
        var result = await _unitOfWork.Complete();

        if (voucher == null) return result < 1 ? null : res.MapOrderToDto();

        voucher.UseCount++;
        await _voucherService.UpdateVoucherCount(voucher);

        return result < 1 ? null : res.MapOrderToDto();
    }

    public async Task<PagedList<OrderEntity>> GetOrders(OrderParams orderParams) {
        var spec = new OrderSpecification(orderParams);
        return await _unitOfWork.Repository<OrderEntity>().GetPagedAsync(spec, orderParams.Page, orderParams.PageSize);
    }

    public async Task<OrderDto?> GetOrder(string orderId) {
        var orderSpec = new OrderSpecification(orderId);
        var existing = await _unitOfWork.Repository<OrderEntity>().GetEntityWithSpec(orderSpec);
        return existing?.MapOrderToDto();
    }
}