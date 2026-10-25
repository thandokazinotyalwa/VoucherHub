using VoucherHub.Modules.Vouchers.Application.Interfaces;
using VoucherHub.Modules.Vouchers.Domain;
using VoucherHub.Modules.Vouchers.Application.DTOs;
using Microsoft.Extensions.Options;
using VoucherHub.Modules.Vouchers.Application.Configuration;

namespace VoucherHub.Modules.Vouchers.Application.Services;

public class VoucherService
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly VoucherSettings _voucherSettings;

    public VoucherService(
        IVoucherRepository voucherRepository,
        IOptions<VoucherSettings> voucherSettings
        )
    {
        _voucherRepository = voucherRepository;
        _voucherSettings = voucherSettings.Value;
    }

    public async Task<List<Voucher>> GetAllVouchersAsync()
{
    var vouchers = await _voucherRepository.GetAllAsync();
    var today = DateOnly.FromDateTime(DateTime.UtcNow);

    foreach (var voucher in vouchers)
    {
        if (voucher.Status == VoucherStatus.Active &&
            voucher.ExpiryDate < today)
        {
            await _voucherRepository.ExpireAsync(voucher.Id);
        }
    }

    return await _voucherRepository.GetAllAsync();
}

    public async Task<Voucher?> GetVoucherByIdAsync(int id)
{
    var voucher = await _voucherRepository.GetByIdAsync(id);

    if (voucher is null)
    {
        return null;
    }

    var today = DateOnly.FromDateTime(DateTime.UtcNow);

    if (voucher.Status == VoucherStatus.Active &&
        voucher.ExpiryDate < today)
    {
        return await _voucherRepository.ExpireAsync(voucher.Id);
    }

    return voucher;
}

    public async Task<Voucher> CreateVoucherAsync(CreateVoucherRequest request)
    {
        if (request.Amount <= 0)
        {
            throw new ArgumentException("Voucher amount must be greater than 0.");
        }

        var voucher = new Voucher
        {
            Code = GenerateCode(),
            Amount = request.Amount,
            Status = VoucherStatus.Active,
            ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow)
    .AddDays(_voucherSettings.ValidityDays)
        };

        return await _voucherRepository.AddAsync(voucher);
    }

    private static string GenerateCode()
    {
        return $"VH-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }

    public async Task<Voucher?> UpdateVoucherAsync(
    int id,
    UpdateVoucherRequest request)
    {
        if (request.Amount <= 0)
        {
            throw new ArgumentException("Voucher amount must be greater than 0.");
        }

        var existingVoucher = await _voucherRepository.GetByIdAsync(id);

        if (existingVoucher is null)
        {
            return null;
        }

        existingVoucher.Amount = request.Amount;

        return await _voucherRepository.UpdateAsync(id, existingVoucher);
    }

    public async Task<Voucher?> CancelVoucherAsync(int id)
    {
        var voucher = await _voucherRepository.GetByIdAsync(id);

        if (voucher is null)
        {
            return null;
        }

        if (voucher.Status != VoucherStatus.Active)
        {
            throw new InvalidOperationException("Only active vouchers can be cancelled.");
        }

        return await _voucherRepository.CancelAsync(id);
    }


    


public async Task<Voucher?> RedeemVoucherAsync(int id)
{
    var voucher = await _voucherRepository.GetByIdAsync(id);

    if (voucher is null)
    {
        return null;
    }

    if (voucher.Status != VoucherStatus.Active)
    {
        throw new InvalidOperationException(
            "Only active vouchers can be redeemed.");
    }

    if (voucher.ExpiryDate < DateOnly.FromDateTime(DateTime.UtcNow))
    {
        await _voucherRepository.ExpireAsync(id);

        throw new InvalidOperationException(
            "Expired vouchers cannot be redeemed.");
    }

    return await _voucherRepository.RedeemAsync(id);
}





}