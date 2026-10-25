
using VoucherHub.Modules.Vouchers.Application.DTOs;
using VoucherHub.Modules.Vouchers.Application.Interfaces;
using VoucherHub.Modules.Vouchers.Application.Services;
using VoucherHub.Modules.Vouchers.Domain;
using Xunit;
using Microsoft.Extensions.Options;
using VoucherHub.Modules.Vouchers.Application.Configuration;

namespace VoucherHub.Tests.Modules.Vouchers.Application;

public class VoucherServiceTests
{
    [Fact]
    public async Task CreateVoucherAsync_WithZeroAmount_ThrowsArgumentException()
    {
        var repository = new TestVoucherRepository();
        var settings = Options.Create(new VoucherSettings
        {
            ValidityDays = 30
        });

        var service = new VoucherService(repository, settings);
        var request = new CreateVoucherRequest { Amount = 0 };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateVoucherAsync(request));
    }

    [Fact]
    public async Task CreateVoucherAsync_WithValidAmount_CreatesActiveVoucher()
    {
        var repository = new TestVoucherRepository();
        var settings = Options.Create(new VoucherSettings
        {
            ValidityDays = 30
        });

        var service = new VoucherService(repository, settings);
        var request = new CreateVoucherRequest { Amount = 500 };

        var voucher = await service.CreateVoucherAsync(request);

        Assert.Equal(500, voucher.Amount);
        Assert.Equal(VoucherStatus.Active, voucher.Status);
        Assert.StartsWith("VH-", voucher.Code);
        Assert.NotEmpty(voucher.Code);
        Assert.Equal(
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
            voucher.ExpiryDate);
    }

    [Fact]
    public async Task RedeemVoucherAsync_WhenVoucherAlreadyRedeemed_ThrowsInvalidOperationException()
    {
        var repository = new TestVoucherRepository
        {
            StoredVoucher = new Voucher
            {
                Id = 1,
                Code = "VH-TEST123",
                Amount = 500,
                Status = VoucherStatus.Redeemed,
                ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10)
            }
        };

        var settings = Options.Create(new VoucherSettings
        {
            ValidityDays = 30
        });

        var service = new VoucherService(repository, settings);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RedeemVoucherAsync(1));
    }


[Fact]
public async Task RedeemVoucherAsync_WhenCancelledVoucherHasExpired_ThrowsInvalidOperationException()
{
    var repository = new TestVoucherRepository
    {
        StoredVoucher = new Voucher
        {
            Id = 1,
            Code = "VH-TEST123",
            Amount = 500,
            Status = VoucherStatus.Cancelled,
            ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1)
        }
    };

    var settings = Options.Create(new VoucherSettings
    {
        ValidityDays = 30
    });

    var service = new VoucherService(repository, settings);

    var exception = await Assert.ThrowsAsync<InvalidOperationException>(
        () => service.RedeemVoucherAsync(1));

    Assert.Equal(
        "Only active vouchers can be redeemed.",
        exception.Message);

    Assert.Equal(VoucherStatus.Cancelled, repository.StoredVoucher.Status);
}


    [Fact]
    public async Task RedeemVoucherAsync_WhenVoucherExpired_ThrowsInvalidOperationException()
    {
        var repository = new TestVoucherRepository
        {
            StoredVoucher = new Voucher
            {
                Id = 1,
                Code = "VH-TEST123",
                Amount = 500,
                Status = VoucherStatus.Active,
                ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1)
            }
        };

        var settings = Options.Create(new VoucherSettings
        {
            ValidityDays = 30
        });

        var service = new VoucherService(repository, settings);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RedeemVoucherAsync(1));

        Assert.Equal(VoucherStatus.Expired, repository.StoredVoucher.Status);
    }


    [Fact]
    public async Task CancelVoucherAsync_WhenVoucherAlreadyRedeemed_ThrowsInvalidOperationException()
    {
        var repository = new TestVoucherRepository
        {
            StoredVoucher = new Voucher
            {
                Id = 1,
                Code = "VH-TEST123",
                Amount = 500,
                Status = VoucherStatus.Redeemed,
                ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10)
            }
        };

        var settings = Options.Create(new VoucherSettings
        {
            ValidityDays = 30
        });

        var service = new VoucherService(repository, settings);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CancelVoucherAsync(1));
        
        Assert.Equal(VoucherStatus.Redeemed, repository.StoredVoucher.Status);
   
    }

    [Fact]
    public async Task GetVoucherByIdAsync_WhenVoucherDoesNotExist_ReturnsNull()
    {
        var repository = new TestVoucherRepository();
        var service = new VoucherService(repository, Options.Create(new VoucherSettings { ValidityDays = 30 }));

        var voucher = await service.GetVoucherByIdAsync(999);

        Assert.Null(voucher);
    }


[Fact]
public async Task GetVoucherByIdAsync_WhenVoucherExpiresToday_RemainsActive()
{
    var repository = new TestVoucherRepository
    {
        StoredVoucher = new Voucher
        {
            Id = 1,
            Code = "VH-TEST789",
            Amount = 500,
            Status = VoucherStatus.Active,
            ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow)
        }
    };

    var settings = Options.Create(new VoucherSettings
    {
        ValidityDays = 30
    });

    var service = new VoucherService(repository, settings);

    var voucher = await service.GetVoucherByIdAsync(1);

    Assert.NotNull(voucher);
    Assert.Equal(VoucherStatus.Active, voucher.Status);
}


    [Fact]
    public async Task UpdateVoucherAsync_WhenVoucherDoesNotExist_ReturnsNull()
    {
        var repository = new TestVoucherRepository();
        var service = new VoucherService(repository, Options.Create(new VoucherSettings { ValidityDays = 30 }));
        var request = new UpdateVoucherRequest { Amount = 800 };

        var voucher = await service.UpdateVoucherAsync(999, request);

        Assert.Null(voucher);
    }

    [Fact]
    public async Task UpdateVoucherAsync_WithZeroAmount_ThrowsArgumentException()
    {
        var repository = new TestVoucherRepository();
        var service = new VoucherService(repository, Options.Create(new VoucherSettings { ValidityDays = 30 }));
        var request = new UpdateVoucherRequest { Amount = 0 };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateVoucherAsync(1, request));
    }

    [Fact]
public async Task GetAllVouchersAsync_WhenVoucherHasExpired_MarksItAsExpired()
{
    var repository = new TestVoucherRepository
    {
        StoredVoucher = new Voucher
        {
            Id = 1,
            Code = "VH-TEST123",
            Amount = 500,
            Status = VoucherStatus.Active,
            ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1)
        }
    };

    var settings = Options.Create(new VoucherSettings
    {
        ValidityDays = 30
    });

    var service = new VoucherService(repository, settings);

    var vouchers = await service.GetAllVouchersAsync();

    Assert.Single(vouchers);
    Assert.Equal(VoucherStatus.Expired, vouchers[0].Status);
}

[Fact]
public async Task GetVoucherByIdAsync_WhenVoucherHasExpired_MarksItAsExpired()
{
    var repository = new TestVoucherRepository
    {
        StoredVoucher = new Voucher
        {
            Id = 1,
            Code = "VH-TEST456",
            Amount = 500,
            Status = VoucherStatus.Active,
            ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1)
        }
    };

    var settings = Options.Create(new VoucherSettings
    {
        ValidityDays = 30
    });

    var service = new VoucherService(repository, settings);

    var voucher = await service.GetVoucherByIdAsync(1);

    Assert.NotNull(voucher);
    Assert.Equal(VoucherStatus.Expired, voucher.Status);
}
    private class TestVoucherRepository : IVoucherRepository
    {
        public Voucher? StoredVoucher { get; set; }

        public Task<Voucher> AddAsync(Voucher voucher)
        {
            StoredVoucher = voucher;
            StoredVoucher.Id = 1;
            return Task.FromResult(StoredVoucher);
        }

        public Task<List<Voucher>> GetAllAsync() =>
            Task.FromResult(
                StoredVoucher is null
                    ? new List<Voucher>()
                    : new List<Voucher> { StoredVoucher });

        public Task<Voucher?> GetByIdAsync(int id) =>
            Task.FromResult(
                StoredVoucher?.Id == id ? StoredVoucher : null);

        public Task<Voucher?> UpdateAsync(int id, Voucher voucher)
        {
            if (StoredVoucher is null || StoredVoucher.Id != id)
            {
                return Task.FromResult<Voucher?>(null);
            }

            StoredVoucher.Amount = voucher.Amount;
            return Task.FromResult<Voucher?>(StoredVoucher);
        }

        public Task<Voucher?> CancelAsync(int id)
        {
            if (StoredVoucher is null || StoredVoucher.Id != id)
            {
                return Task.FromResult<Voucher?>(null);
            }

            StoredVoucher.Status = VoucherStatus.Cancelled;
            return Task.FromResult<Voucher?>(StoredVoucher);
        }

        public Task<Voucher?> RedeemAsync(int id)
        {
            if (StoredVoucher is null || StoredVoucher.Id != id)
            {
                return Task.FromResult<Voucher?>(null);
            }

            StoredVoucher.Status = VoucherStatus.Redeemed;
            return Task.FromResult<Voucher?>(StoredVoucher);
        }

        public Task<Voucher?> ExpireAsync(int id)
        {
            if (StoredVoucher is null || StoredVoucher.Id != id)
            {
                return Task.FromResult<Voucher?>(null);
            }

            StoredVoucher.Status = VoucherStatus.Expired;
            return Task.FromResult<Voucher?>(StoredVoucher);
        }
    }
}
