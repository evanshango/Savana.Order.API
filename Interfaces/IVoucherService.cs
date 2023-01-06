using Savana.Order.API.Dtos;
using Savana.Order.API.Entities;
using Savana.Order.API.Requests;
using Savana.Order.API.Requests.Params;
using Treasures.Common.Helpers;

namespace Savana.Order.API.Interfaces; 

public interface IVoucherService {
    Task<PagedList<VoucherEntity>> GetVouchers(VoucherParams voucherParams);
    Task<VoucherEntity?> GetVoucher(string voucher);
    Task<VoucherDto?> DeleteVoucher(VoucherEntity existing, string updatedBy);
    Task<VoucherDto?> AddVoucher(VoucherReq voucherReq, string createdBy);
    Task<VoucherDto?> UpdateVoucher(VoucherEntity existing, VoucherReq voucherReq, string updatedBy);
    Task<VoucherDto?> UpdateVoucherCount(VoucherEntity existing);
}