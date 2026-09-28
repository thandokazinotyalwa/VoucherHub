using VoucherHub.Modules.Vouchers.Domain;

namespace VoucherHub.Modules.Vouchers.Application.Interfaces;

public interface IVoucherRepository
{
    Task<Voucher> AddAsync(Voucher voucher);

    Task<Voucher?> UpdateAsync(int id, Voucher voucher);
    Task<List<Voucher>> GetAllAsync();
    Task<Voucher?> GetByIdAsync(int id);

}