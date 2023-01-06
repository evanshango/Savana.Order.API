using System.ComponentModel.DataAnnotations;

namespace Savana.Order.API.Requests;

public class VoucherReq {
    [Required(ErrorMessage = "Title is required")]
    public string? Title { get; set; }

    [Required, Range(1, 100, ErrorMessage = "Value should be between 1 and 100")]
    public double Discount { get; set; }

    [Required(ErrorMessage = "ExpiresAfter is required")]
    public ExpiresAfter? ExpiresAfter { get; set; }

    [Required] public int MaxUse { get; set; }
    public int UseCount { get; set; } = 0;
}

public class ExpiresAfter {
    [Required(ErrorMessage = "Type is required(days, hours)")]
    public string? Type { get; set; }

    [Required(ErrorMessage = "A value is required")]
    public double Value { get; set; }
}