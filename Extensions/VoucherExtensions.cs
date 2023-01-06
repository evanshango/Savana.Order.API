using Savana.Order.API.Dtos;
using Savana.Order.API.Entities;

namespace Savana.Order.API.Extensions; 

public static class VoucherExtensions {
    public static VoucherDto MapVoucherToDto(this VoucherEntity voucher) => new() {
        Title = voucher.Title, Voucher = voucher.Voucher, Discount = voucher.Discount, ExpiresOn = voucher.ExpiresOn,
        MaxUse = voucher.MaxUse, UseCount = voucher.UseCount, CreatedAt = voucher.CreatedAt
    };
}