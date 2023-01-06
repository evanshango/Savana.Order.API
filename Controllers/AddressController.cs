using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savana.Order.API.Dtos;
using Savana.Order.API.Interfaces;
using Savana.Order.API.Requests;
using Savana.Order.API.Requests.Params;
using Treasures.Common.Extensions;
using Treasures.Common.Helpers;

namespace Savana.Order.API.Controllers;

[ApiController, Route("addresses"), Produces("application/json"), Tags("Addresses")]
public class AddressController : ControllerBase {
    private readonly IAddressService _addressService;
    public AddressController(IAddressService addressService) => _addressService = addressService;

    [HttpGet(""), Authorize]
    public async Task<ActionResult<AddressDto>> GetUserAddress([FromQuery] AddressParams addressParams) {
        var address = await _addressService.GetUserAddress(addressParams);
        return address != null ? Ok(address) : NotFound(new ApiException(404, "User address not found"));
    }

    [HttpPost, Authorize]
    public async Task<ActionResult<AddressDto>> AddAddress([FromBody] AddressReq addressReq) {
        var res = await _addressService.AddAddress(addressReq, User.RetrieveEmailFromPrincipal());
        return res != null ? Ok(res) : BadRequest(new ApiException(400, "Unable to add a new address"));
    }

    [HttpDelete, Authorize]
    public async Task<IActionResult> DeleteAddress() {
        var updatedBy = User.RetrieveEmailFromPrincipal();
        var existingAddress = await _addressService.GetAddress(updatedBy);

        if (existingAddress == null) return NotFound(new ApiException(404, "User Address not found"));

        if (!existingAddress.Email!.ToLower().Equals(updatedBy.ToLower().Trim()))
            return Unauthorized(new ApiException(403, "Operation not allowed"));

        var res = await _addressService.DeleteAddress(existingAddress, updatedBy);
        return res != null ? NoContent() : BadRequest(new ApiException(400, "Unable to remove address"));
    }
}