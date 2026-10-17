
using VoucherHub.Modules.Vouchers.Application.DTOs;
using VoucherHub.Modules.Vouchers.Application.Interfaces;
using VoucherHub.Modules.Vouchers.Application.Services;
using VoucherHub.Modules.Vouchers.Domain;
using Xunit;

namespace VoucherHub.Tests.Modules.Vouchers.Application;

public class VoucherServiceTests
{
    [Fact]
    public async Task CreateVoucherAsync_WithZeroAmount_ThrowsArgumentException()
    {
        // Arrange
        IVoucherRepository repository = new TestVoucherRepository();
        var service = new VoucherService(repository);
        var request = new CreateVoucherRequest { Amount = 0 };

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateVoucherAsync(request));
    }


    [Fact]
    public async Task CreateVoucherAsync_WithValidAmount_CreatesActiveVoucher()
    {
        // Arrange
        IVoucherRepository repository = new TestVoucherRepository();
        var service = new VoucherService(repository);
        var request = new CreateVoucherRequest { Amount = 500 };

        // Act
        var voucher = await service.CreateVoucherAsync(request);

        // Assert
        Assert.Equal(500, voucher.Amount);
        Assert.Equal(VoucherStatus.Active, voucher.Status);
        Assert.StartsWith("VH-", voucher.Code);
        Assert.NotEmpty(voucher.Code);
    }

    [Fact]
    public async Task RedeemVoucherAsync_WhenVoucherAlreadyRedeemed_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new TestVoucherRepository
        {
            StoredVoucher = new Voucher
            {
                Id = 1,
                Code = "VH-TEST123",
                Amount = 500,
                Status = VoucherStatus.Redeemed
            }
        };

        var service = new VoucherService(repository);

        // Act and Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RedeemVoucherAsync(1));
    }

    [Fact]
public async Task CancelVoucherAsync_WhenVoucherAlreadyRedeemed_ThrowsInvalidOperationException()
{
    // Arrange
    var repository = new TestVoucherRepository
    {
        StoredVoucher = new Voucher
        {
            Id = 1,
            Code = "VH-TEST123",
            Amount = 500,
            Status = VoucherStatus.Redeemed
        }
    };

    var service = new VoucherService(repository);

    // Act and Assert
    await Assert.ThrowsAsync<InvalidOperationException>(
        () => service.CancelVoucherAsync(1));
}

[Fact]
public async Task GetVoucherByIdAsync_WhenVoucherDoesNotExist_ReturnsNull()
{
    // Arrange
    var repository = new TestVoucherRepository();
    var service = new VoucherService(repository);

    // Act
    var voucher = await service.GetVoucherByIdAsync(999);

    // Assert
    Assert.Null(voucher);
}


[Fact]
public async Task UpdateVoucherAsync_WhenVoucherDoesNotExist_ReturnsNull()
{
    // Arrange
    var repository = new TestVoucherRepository();
    var service = new VoucherService(repository);
    var request = new UpdateVoucherRequest { Amount = 800 };

    // Act
    var voucher = await service.UpdateVoucherAsync(999, request);

    // Assert
    Assert.Null(voucher);
}


[Fact]
public async Task UpdateVoucherAsync_WithZeroAmount_ThrowsArgumentException()
{
    // Arrange
    var repository = new TestVoucherRepository();
    var service = new VoucherService(repository);
    var request = new UpdateVoucherRequest { Amount = 0 };

    // Act and Assert
    await Assert.ThrowsAsync<ArgumentException>(
        () => service.UpdateVoucherAsync(1, request));
}


[Fact]
public async Task RedeemVoucherAsync_WhenVoucherDoesNotExist_ReturnsNull()
{
    // Arrange
    var repository = new TestVoucherRepository();
    var service = new VoucherService(repository);

    // Act
    var voucher = await service.RedeemVoucherAsync(999);

    // Assert
    Assert.Null(voucher);
}


[Fact]
public async Task CancelVoucherAsync_WhenVoucherDoesNotExist_ReturnsNull()
{
    // Arrange
    var repository = new TestVoucherRepository();
    var service = new VoucherService(repository);

    // Act
    var voucher = await service.CancelVoucherAsync(999);

    // Assert
    Assert.Null(voucher);
}

    private class TestVoucherRepository : IVoucherRepository
    {
        public Task<Voucher> AddAsync(Voucher voucher) =>
            Task.FromResult(voucher);

        public Task<List<Voucher>> GetAllAsync() =>
            Task.FromResult(new List<Voucher>());

        public Task<Voucher?> GetByIdAsync(int id) =>
    Task.FromResult(
        StoredVoucher?.Id == id ? StoredVoucher : null);

        public Task<Voucher?> UpdateAsync(int id, Voucher voucher) =>
            Task.FromResult<Voucher?>(null);

        public Task<Voucher?> CancelAsync(int id) =>
            Task.FromResult<Voucher?>(null);

        public Task<Voucher?> RedeemAsync(int id)
        {
            if (StoredVoucher is null || StoredVoucher.Id != id)
            {
                return Task.FromResult<Voucher?>(null);
            }

            StoredVoucher.Status = VoucherStatus.Redeemed;

            return Task.FromResult<Voucher?>(StoredVoucher);
        }

        public Voucher? StoredVoucher { get; set; }
    }



}