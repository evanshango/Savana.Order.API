using System.ComponentModel.DataAnnotations;
using Treasures.Common.Helpers;

namespace Savana.Order.API.Entities;

public class VoucherEntity : BaseEntity {
    [Required] public string? Title { get; set; }
    [Required] public string? Voucher { get; set; }

    [Required, Range(1, 100, ErrorMessage = "Value should be between 1 and 100")]
    public double Discount { get; set; }

    [Required] public DateTime ExpiresOn { get; set; }
    [Required] public int MaxUse { get; set; }
    [Required] public int UseCount { get; set; }

    public static string GenerateVoucher() {
        var voucher = Guid.NewGuid().ToString("N")[..13].ToUpper();
        return $"{voucher[..4]}-{voucher[4..8]}-{voucher[8..]}";
    }
}