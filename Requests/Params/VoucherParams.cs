using Treasures.Common.Helpers;

namespace Savana.Order.API.Requests.Params;

public class VoucherParams : Pagination {
    public string? Voucher { get; set; }
    public bool Enabled { get; set; } = true;
}