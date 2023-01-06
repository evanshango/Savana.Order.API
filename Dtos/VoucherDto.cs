namespace Savana.Order.API.Dtos;

public class VoucherDto {
    public string? Title { get; set; }
    public string? Voucher { get; set; }
    public double Discount { get; set; }
    public DateTime ExpiresOn { get; set; }
    public int MaxUse { get; set; }
    public int UseCount { get; set; }
    public DateTime CreatedAt { get; set; }
}