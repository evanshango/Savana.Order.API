using Savana.Order.API.Dtos;
using Savana.Order.API.Entities;
using Savana.Order.API.Extensions;
using Savana.Order.API.Interfaces;
using Savana.Order.API.Requests;
using Savana.Order.API.Requests.Params;
using Savana.Order.API.Specification;
using Treasures.Common.Helpers;
using Treasures.Common.Interfaces;

namespace Savana.Order.API.Services;

public class VoucherService : IVoucherService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VoucherService> _logger;

    public VoucherService(IUnitOfWork unitOfWork, ILogger<VoucherService> logger) {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedList<VoucherEntity>> GetVouchers(VoucherParams vParams) {
        var spec = new VoucherSpecification(vParams);
        return await _unitOfWork.Repository<VoucherEntity>().GetPagedAsync(spec, vParams.Page, vParams.PageSize);
    }

    public async Task<VoucherEntity?> GetVoucher(string voucher) {
        var spec = new VoucherSpecification(voucher);
        return await _unitOfWork.Repository<VoucherEntity>().GetEntityWithSpec(spec);
    }

    public async Task<VoucherDto?> DeleteVoucher(VoucherEntity existing, string updatedBy) {
        existing.Active = false;
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;
        return await SaveVoucherChanges(existing);
    }

    public async Task<VoucherDto?> AddVoucher(VoucherReq voucherReq, string createdBy) {
        var newVoucher = new VoucherEntity {
            Title = voucherReq.Title, Discount = voucherReq.Discount, CreatedBy = createdBy, MaxUse = voucherReq.MaxUse,
            ExpiresOn = GetExpiryTime(voucherReq.ExpiresAfter), UseCount = 0,
            Voucher = VoucherEntity.GenerateVoucher()
        };

        var res = _unitOfWork.Repository<VoucherEntity>().AddAsync(newVoucher);
        var result = await _unitOfWork.Complete();

        if (result >= 1) {
            _logger.LogInformation("Voucher with title {Title} created", res.Title);
            return res.MapVoucherToDto();
        }

        _logger.LogError("Unable to create voucher with title {Title}", voucherReq.Title);
        return null;
    }

    public async Task<VoucherDto?> UpdateVoucher(VoucherEntity existing, VoucherReq voucherReq, string updatedBy) {
        existing.Title = voucherReq.Title ?? existing.Title;
        existing.Discount = voucherReq.Discount;
        existing.MaxUse = voucherReq.MaxUse;
        existing.UseCount = voucherReq.UseCount;
        existing.ExpiresOn = GetExpiryTime(voucherReq.ExpiresAfter);
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;
        return await SaveVoucherChanges(existing);
    }

    public async Task<VoucherDto?> UpdateVoucherCount(VoucherEntity existing) => await SaveVoucherChanges(existing);

    private async Task<VoucherDto?> SaveVoucherChanges(VoucherEntity existing) {
        var res = _unitOfWork.Repository<VoucherEntity>().UpdateAsync(existing);
        var result = await _unitOfWork.Complete();
        if (result >= 1) {
            _logger.LogInformation("Voucher with title {Title} updated", res.Title);
            return res.MapVoucherToDto();
        }

        _logger.LogError("Unable to update voucher with title {Title}", existing.Title);
        return null;
    }

    private static DateTime GetExpiryTime(ExpiresAfter? expiresAfter) {
        var expiresAt = DateTime.UtcNow.AddHours(1);
        if (expiresAfter == null) return expiresAt;

        var expType = expiresAfter.Type;
        var duration = expiresAfter.Value;
        if (!string.IsNullOrEmpty(expType)) {
            expiresAt = expType switch {
                "days" => DateTime.UtcNow.AddDays(duration),
                "hours" => DateTime.UtcNow.AddHours(duration),
                _ => DateTime.UtcNow.AddDays(1)
            };
        }

        return expiresAt;
    }
}