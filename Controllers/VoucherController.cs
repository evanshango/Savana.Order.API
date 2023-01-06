using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savana.Order.API.Dtos;
using Savana.Order.API.Extensions;
using Savana.Order.API.Interfaces;
using Savana.Order.API.Requests;
using Savana.Order.API.Requests.Params;
using Treasures.Common.Extensions;
using Treasures.Common.Helpers;

namespace Savana.Order.API.Controllers;

[ApiController, Route("vouchers"), Produces("application/json"), Tags("Vouchers")]
public class VoucherController : ControllerBase {
    private readonly IVoucherService _voucherService;
    public VoucherController(IVoucherService voucherService) => _voucherService = voucherService;

    [HttpGet("")]
    public async Task<ActionResult<VoucherDto>> GetVouchers([FromQuery] VoucherParams voucherParams) {
        var vouchers = await _voucherService.GetVouchers(voucherParams);
        Response.AddPaginationHeader(vouchers.MetaData);
        return Ok(vouchers.Select(v => v.MapVoucherToDto()).ToList());
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult<VoucherDto>> AddVoucher([FromBody] VoucherReq voucherReq) {
        var createdBy = User.RetrieveEmailFromPrincipal();
        var res = await _voucherService.AddVoucher(voucherReq, createdBy);
        return res != null ? Ok(res) : BadRequest(new ApiException(400, "Unable to create voucher"));
    }

    [HttpPut, Authorize(Roles = "Admin")]
    public async Task<ActionResult<VoucherDto>> UpdateVoucher([FromQuery] string voucher, [FromBody] VoucherReq req) {
        var updatedBy = User.RetrieveEmailFromPrincipal();

        var existing = await _voucherService.GetVoucher(voucher);
        if (existing == null) return NotFound(new ApiException(404, "Voucher not found"));

        var res = await _voucherService.UpdateVoucher(existing, req, updatedBy);
        return res != null ? Ok(res) : BadRequest(new ApiException(400, "Unable to update voucher"));
    }

    [HttpDelete, Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteVoucher([FromQuery] string voucher) {
        var updatedBy = User.RetrieveEmailFromPrincipal();
        
        var existing = await _voucherService.GetVoucher(voucher);
        if (existing == null) return NotFound(new ApiException(404, "Voucher not found"));

        var res = await _voucherService.DeleteVoucher(existing, updatedBy);
        return res != null ? NoContent() : BadRequest(new ApiException(400, "Unable to delete voucher"));
    }
}