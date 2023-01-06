using Savana.Order.API.Entities;
using Savana.Order.API.Requests.Params;
using Treasures.Common.Services;

namespace Savana.Order.API.Specification;

public class VoucherSpecification : SpecificationService<VoucherEntity> {
    public VoucherSpecification(VoucherParams vParams) : base(v =>
        (string.IsNullOrEmpty(vParams.Voucher) || v.Voucher!.ToLower().Equals(vParams.Voucher.ToLower().Trim())) &&
        v.Active == vParams.Enabled
    ) { }

    public VoucherSpecification(string voucher) : base(v =>
        v.Voucher!.ToLower().Equals(voucher.ToLower().Trim())
    ) { }
}