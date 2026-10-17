
using Microsoft.EntityFrameworkCore;
using VoucherHub.Modules.Vouchers.Domain;

namespace VoucherHub.Modules.Vouchers.Infrastructure.Persistence;

public class VoucherHubDbContext : DbContext
{
    public VoucherHubDbContext(
        DbContextOptions<VoucherHubDbContext> options)
        : base(options)
    {
    }

    public DbSet<Voucher> Vouchers => Set<Voucher>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Voucher>()
            .Property(voucher => voucher.Status)
            .HasConversion<string>();
    }
}