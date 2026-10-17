
using Microsoft.EntityFrameworkCore;
using VoucherHub.Modules.Vouchers.Application.Interfaces;
using VoucherHub.Modules.Vouchers.Domain;
using VoucherHub.Modules.Vouchers.Infrastructure.Persistence;

namespace VoucherHub.Modules.Vouchers.Infrastructure.Repositories;

public class PostgresVoucherRepository : IVoucherRepository
{
    private readonly VoucherHubDbContext _dbContext;

    public PostgresVoucherRepository(VoucherHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Voucher> AddAsync(Voucher voucher)
    {
        await _dbContext.Vouchers.AddAsync(voucher);
        await _dbContext.SaveChangesAsync();

        return voucher;
    }

    public async Task<List<Voucher>> GetAllAsync()
    {
        return await _dbContext.Vouchers.ToListAsync();
    }

    public async Task<Voucher?> GetByIdAsync(int id)
    {
        return await _dbContext.Vouchers.FindAsync(id);
    }

    public async Task<Voucher?> UpdateAsync(int id, Voucher voucher)
    {
        var existingVoucher = await _dbContext.Vouchers.FindAsync(id);

        if (existingVoucher is null)
        {
            return null;
        }

        existingVoucher.Amount = voucher.Amount;
        existingVoucher.Code = voucher.Code;
        existingVoucher.Status = voucher.Status;

        await _dbContext.SaveChangesAsync();

        return existingVoucher;
    }

    public async Task<Voucher?> CancelAsync(int id)
    {
        var voucher = await _dbContext.Vouchers.FindAsync(id);

        if (voucher is null)
        {
            return null;
        }

        voucher.Status = VoucherStatus.Cancelled;

        await _dbContext.SaveChangesAsync();

        return voucher;
    }

    public async Task<Voucher?> RedeemAsync(int id)
    {
        var voucher = await _dbContext.Vouchers.FindAsync(id);

        if (voucher is null)
        {
            return null;
        }

        voucher.Status = VoucherStatus.Redeemed;

        await _dbContext.SaveChangesAsync();

        return voucher;
    }
}