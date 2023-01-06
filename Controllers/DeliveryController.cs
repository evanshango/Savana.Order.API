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

[ApiController, Route("delivery/methods"), Produces("application/json"), Tags("Delivery Methods")]
public class DeliveryController : ControllerBase {
    private readonly IDeliveryService _deliveryService;

    public DeliveryController(IDeliveryService deliveryService) => _deliveryService = deliveryService;

    [HttpGet("")]
    public async Task<ActionResult<PagedList<DeliveryDto>>> GetDeliveryMethods([FromQuery] DeliveryParams delParams) {
        var deliveries = await _deliveryService.GetDeliveries(delParams);
        Response.AddPaginationHeader(deliveries.MetaData);
        return Ok(deliveries.Select(d => d.MapDeliveryToDto()).ToList());
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult<DeliveryDto>> AddDeliveryMethod([FromBody] DeliveryReq req) {
        var res = await _deliveryService.AddDeliveryMethod(req, User.RetrieveEmailFromPrincipal());
        return res != null ? Ok(res) : BadRequest(new ApiException(400, "Unable to add  delivery method"));
    }

    [HttpPut("{methodId}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<DeliveryDto>> UpdateDeliveryMethod(
        [FromRoute] string methodId, [FromBody] DeliveryReq deliveryReq
    ) {
        var updatedBy = User.RetrieveEmailFromPrincipal();
        
        var existing = await _deliveryService.GetDeliveryMethod(methodId);
        if (existing == null) return NotFound(new ApiException(404, "Delivery Method not found"));

        var res = await _deliveryService.UpdateDeliveryMethod(existing, deliveryReq, updatedBy);
        return res != null ? Ok(res) : BadRequest(new ApiException(400, "Unable to update delivery method"));
    }

    [HttpDelete("{methodId}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDeliveryMethod([FromRoute] string methodId) {
        var updatedBy = User.RetrieveEmailFromPrincipal();

        var existing = await _deliveryService.GetDeliveryMethod(methodId);
        if (existing == null) return NotFound(new ApiException(404, "Delivery Method not found"));

        var res = await _deliveryService.DeleteDeliveryMethod(existing, updatedBy);
        return res != null ? NoContent() : BadRequest(new ApiException(400, "Unable to delete delivery method"));
    }
}