using System.ComponentModel.DataAnnotations;

namespace Savana.Order.API.Requests;

public class AddressReq {
    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Email is required"), EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Street is required")]
    public string? Street { get; set; }

    public string? Building { get; set; }
    public string? ZipCode { get; set; }

    [Required(ErrorMessage = "City/Town is required")]
    public string? City { get; set; }

    [Required(ErrorMessage = "Country is required")]
    public string? Country { get; set; }
}