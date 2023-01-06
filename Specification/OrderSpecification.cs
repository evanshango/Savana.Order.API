using Savana.Order.API.Entities;
using Savana.Order.API.Requests.Params;
using Treasures.Common.Services;

namespace Savana.Order.API.Specification;

public class OrderSpecification : SpecificationService<OrderEntity> {
    public OrderSpecification(OrderParams orderParams) : base(o =>
        string.IsNullOrEmpty(orderParams.Email) || o.BuyerEmail!.ToLower().Equals(orderParams.Email.ToLower().Trim())
    ) { }

    public OrderSpecification(string orderId) : base(o => o.Id.Equals(orderId)) {
        AddInclude(o => o.Address!);
        AddInclude(o => o.DeliveryMethod!);
        AddInclude(o => o.OrderItems!);
    }
}