using Savana.Order.API.Entities;
using Savana.Order.API.Requests.Params;
using Treasures.Common.Services;

namespace Savana.Order.API.Specification;

public class AddressSpecification : SpecificationService<AddressEntity> {
    public AddressSpecification(AddressParams addParams) : base(a =>
        (string.IsNullOrEmpty(addParams.Name) || a.Name!.ToLower().Contains(addParams.Name.ToLower().Trim())) &&
        (string.IsNullOrEmpty(addParams.Email) || a.Email!.ToLower().Equals(addParams.Email.ToLower().Trim())) ||
        a.Active == addParams.Enabled
    ) { }

    public AddressSpecification(string email) : base(a =>
        a.Email!.ToLower().Equals(email.ToLower().Trim())
    ) { }
}