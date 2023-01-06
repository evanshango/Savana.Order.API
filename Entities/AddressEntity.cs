using System.ComponentModel.DataAnnotations;
using Treasures.Common.Helpers;

namespace Savana.Order.API.Entities;

public class AddressEntity : BaseEntity {
    [Required] public string? Name { get; set; }
    [Required] public string? Email { get; set; }
    [Required] public string? Phone { get; set; }
    [Required] public string? Street { get; set; }
    public string? Building { get; set; }
    public string? ZipCode { get; set; }
    [Required] public string? City { get; set; }
    [Required] public string? Country { get; set; }
}