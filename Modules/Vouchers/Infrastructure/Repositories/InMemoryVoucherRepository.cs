using VoucherHub.Modules.Vouchers.Application.Interfaces;
using VoucherHub.Modules.Vouchers.Domain;

namespace VoucherHub.Modules.Vouchers.Infrastructure.Repositories;

public class InMemoryVoucherRepository : IVoucherRepository
{
    private readonly List<Voucher> _vouchers = new();

    public Task<Voucher> AddAsync(Voucher voucher)
    {
        voucher.Id = _vouchers.Count + 1;
        _vouchers.Add(voucher);

        return Task.FromResult(voucher);
    }

    public Task<Voucher?> UpdateAsync(int id, Voucher voucher)
    {
        var existingVoucher = _vouchers.FirstOrDefault(v => v.Id == id);

        if (existingVoucher == null)
        {
            return Task.FromResult<Voucher?>(null);
        }

        existingVoucher.Amount = voucher.Amount;


        return Task.FromResult<Voucher?>(existingVoucher);
    }

    public Task<List<Voucher>> GetAllAsync()
    {
        return Task.FromResult(_vouchers);
    }

    public Task<Voucher?> GetByIdAsync(int id)
    {
      var voucher = _vouchers.FirstOrDefault(v => v.Id == id);

      return Task.FromResult(voucher);
    }
}