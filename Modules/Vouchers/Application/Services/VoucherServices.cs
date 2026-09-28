using VoucherHub.Modules.Vouchers.Application.Interfaces;
using VoucherHub.Modules.Vouchers.Domain;
using VoucherHub.Modules.Vouchers.Application.DTOs;

namespace VoucherHub.Modules.Vouchers.Application.Services;

public class VoucherService
{
    private readonly IVoucherRepository _voucherRepository;

    public VoucherService(IVoucherRepository voucherRepository)
    {
        _voucherRepository = voucherRepository;
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
            Status = VoucherStatus.Active
        };

        return await _voucherRepository.AddAsync(voucher);
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

    public async Task<List<Voucher>> GetAllVouchersAsync()
    {
        return await _voucherRepository.GetAllAsync();
    }

    public async Task<Voucher?> GetVoucherByIdAsync(int id)
    {
        return await _voucherRepository.GetByIdAsync(id);
    }

    private static string GenerateCode()
    {
        return $"VH-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }

}