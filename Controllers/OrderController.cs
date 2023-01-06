using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savana.Order.API.Dtos;
using Savana.Order.API.Entities;
using Savana.Order.API.Extensions;
using Savana.Order.API.Interfaces;
using Savana.Order.API.Requests;
using Savana.Order.API.Requests.Params;
using Treasures.Common.Extensions;
using Treasures.Common.Helpers;

namespace Savana.Order.API.Controllers;

[ApiController, Route("orders"), Produces("application/json"), Tags("Orders")]
public class OrderController : ControllerBase {
    private readonly IOrderService _orderService;
    private readonly IVoucherService _voucherService;
    private readonly IDeliveryService _deliveryService;
    private readonly IAddressService _addressService;

    public OrderController(
        IOrderService orderService, IVoucherService vService, IDeliveryService dService, IAddressService aService
    ) {
        _orderService = orderService;
        _voucherService = vService;
        _deliveryService = dService;
        _addressService = aService;
    }

    [HttpGet, Authorize]
    public async Task<ActionResult<PagedList<OrderDto>>> GetOrders([FromQuery] OrderParams orderParams) {
        var deliveries = await _orderService.GetOrders(orderParams);
        Response.AddPaginationHeader(deliveries.MetaData);
        return Ok(deliveries.Select(o => o.MapOrderToDto()).ToList());
    }

    [HttpGet("{orderId}"), Authorize]
    public async Task<ActionResult<OrderDto>> GetOrder([FromRoute] string orderId) {
        var order = await _orderService.GetOrder(orderId);
        return order != null ? Ok(order) : NotFound(new ApiException(404, "Order not found"));
    }

    [HttpPost, Authorize]
    public async Task<ActionResult<OrderDto>> AddOrder([FromBody] OrderReq orderReq) {
        var createdBy = User.RetrieveEmailFromPrincipal();

        if (string.IsNullOrEmpty(orderReq.DeliveryId)) {
            return BadRequest(new ApiException(400, "Please select a delivery method"));
        }

        if (string.IsNullOrEmpty(orderReq.AddressId)) {
            return BadRequest(new ApiException(400, "Please select your Preferred address"));
        }

        VoucherEntity? voucher = null;
        if (!string.IsNullOrEmpty(orderReq.VoucherCode)) {
            voucher = await _voucherService.GetVoucher(orderReq.VoucherCode);
        }

        if (voucher != null && (voucher.UseCount == voucher.MaxUse || DateTime.UtcNow > voucher.ExpiresOn)) {
            return BadRequest(new ApiException(400, "Invalid voucher code"));
        }

        var existingDelivery = await _deliveryService.GetDeliveryMethod(orderReq.DeliveryId);
        var existingAddress = await _addressService.GetAddressById(orderReq.AddressId);

        if (existingDelivery == null) {
            return BadRequest(new ApiException(400, "Delivery method not found"));
        }
        
        if (existingAddress == null) {
            return BadRequest(new ApiException(400, "Delivery address not found"));
        }
        
        if (!existingDelivery.Active) {
            return BadRequest(new ApiException(400, "Delivery method not found"));
        }
        
        if (!existingAddress.Active) {
            return BadRequest(new ApiException(400, "Delivery address not found"));
        }
        
        var res = await _orderService.CreateOrder(
            orderReq, voucher, existingDelivery, existingAddress, createdBy
        );
        return res != null ? Ok(res) : BadRequest(new ApiException(400, "Unable to create order"));
    }
}