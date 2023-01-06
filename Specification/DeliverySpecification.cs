using Savana.Order.API.Entities;
using Savana.Order.API.Requests.Params;
using Treasures.Common.Services;

namespace Savana.Order.API.Specification;

public class DeliverySpecification : SpecificationService<DeliveryEntity> {
    public DeliverySpecification(DeliveryParams delParams) : base(d =>
        string.IsNullOrEmpty(delParams.Title) || d.Title!.ToLower().Contains(delParams.Title.ToLower().Trim()) &&
        d.Active == delParams.Enabled
    ) { }
}