using System.Net;
using System.Net.Http.Json;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using VoucherHub.Modules.Vouchers.Domain;
using VoucherHub.Modules.Vouchers.Infrastructure.Persistence;


namespace VoucherHub.Tests.Modules.Vouchers.API;

public class VoucherEndpointsTests
    : IClassFixture<VoucherHubWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly VoucherHubWebApplicationFactory _factory;

    public VoucherEndpointsTests(
        VoucherHubWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetVoucherById_WhenVoucherDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/vouchers/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateVoucher_WithValidAmount_ReturnsCreatedVoucher()
    {
        // Arrange
        var request = new
        {
            Amount = 500m
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/vouchers",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var voucher = await response.Content.ReadFromJsonAsync<VoucherResponse>();

        Assert.NotNull(voucher);
        Assert.True(voucher.Id > 0);
        Assert.Equal(500m, voucher.Amount);
        Assert.Equal("Active", voucher.Status);
        Assert.StartsWith("VH-", voucher.Code);
    }

    

    [Fact]
    public async Task CreateVoucher_WithZeroAmount_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            Amount = 0m
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/vouchers",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


[Fact]
public async Task CreateVoucher_WithNegativeAmount_ReturnsBadRequest()
{
    var request = new
    {
        Amount = -100m
    };

    var response = await _client.PostAsJsonAsync(
        "/api/vouchers",
        request);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
}


[Fact]
public async Task CreateVoucher_WithAmountAboveMaximum_ReturnsBadRequest()
{
    var request = new
    {
        Amount = 1_000_000_000m
    };

    var response = await _client.PostAsJsonAsync(
        "/api/vouchers",
        request);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
}


[Fact]
public async Task UpdateVoucher_WithValidAmount_ReturnsUpdatedVoucher()
{
    // Arrange: create a voucher
    var createResponse = await _client.PostAsJsonAsync(
        "/api/vouchers",
        new { Amount = 250m });

    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

    var voucher = await createResponse.Content
        .ReadFromJsonAsync<VoucherResponse>();

    Assert.NotNull(voucher);

    // Act: update the voucher amount
    var updateResponse = await _client.PutAsJsonAsync(
        $"/api/vouchers/{voucher.Id}",
        new { Amount = 400m });

    // Assert: verify the updated voucher
    Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

    var updatedVoucher = await updateResponse.Content
        .ReadFromJsonAsync<VoucherResponse>();

    Assert.NotNull(updatedVoucher);
    Assert.Equal(voucher.Id, updatedVoucher.Id);
    Assert.Equal(400m, updatedVoucher.Amount);
    Assert.Equal("Active", updatedVoucher.Status);
}


[Fact]
public async Task UpdateVoucher_WithZeroAmount_ReturnsBadRequest()
{
    // Arrange: create a voucher
    var createResponse = await _client.PostAsJsonAsync(
        "/api/vouchers",
        new { Amount = 250m });

    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

    var voucher = await createResponse.Content
        .ReadFromJsonAsync<VoucherResponse>();

    Assert.NotNull(voucher);

    // Act: attempt to update with an invalid amount
    var updateResponse = await _client.PutAsJsonAsync(
        $"/api/vouchers/{voucher.Id}",
        new { Amount = 0m });

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
}


[Fact]
public async Task UpdateVoucher_WithNegativeAmount_ReturnsBadRequest()
{
    // Arrange: create a voucher
    var createResponse = await _client.PostAsJsonAsync(
        "/api/vouchers",
        new { Amount = 250m });

    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

    var voucher = await createResponse.Content
        .ReadFromJsonAsync<VoucherResponse>();

    Assert.NotNull(voucher);

    // Act: attempt to update with a negative amount
    var updateResponse = await _client.PutAsJsonAsync(
        $"/api/vouchers/{voucher.Id}",
        new { Amount = -100m });

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
}


[Fact]
public async Task UpdateVoucher_WithAmountAboveMaximum_ReturnsBadRequest()
{
    // Arrange: create a voucher
    var createResponse = await _client.PostAsJsonAsync(
        "/api/vouchers",
        new { Amount = 250m });

    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

    var voucher = await createResponse.Content
        .ReadFromJsonAsync<VoucherResponse>();

    Assert.NotNull(voucher);

    // Act: attempt to update above the maximum
    var updateResponse = await _client.PutAsJsonAsync(
        $"/api/vouchers/{voucher.Id}",
        new { Amount = 1_000_000_000m });

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
}


[Fact]
public async Task UpdateVoucher_WhenVoucherDoesNotExist_ReturnsNotFound()
{
    var response = await _client.PutAsJsonAsync(
        "/api/vouchers/999",
        new { Amount = 250m });

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
}



    [Fact]
    public async Task RedeemVoucher_WhenVoucherIsActive_ReturnsRedeemedVoucher()
    {
        // Arrange: create a voucher
        var createResponse = await _client.PostAsJsonAsync(
            "/api/vouchers",
            new { Amount = 250m });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdVoucher =
            await createResponse.Content.ReadFromJsonAsync<VoucherResponse>();

        Assert.NotNull(createdVoucher);

        // Act: redeem the voucher
        var redeemResponse = await _client.PostAsync(
            $"/api/vouchers/{createdVoucher.Id}/redeem",
            content: null);

        // Assert: verify the response
        Assert.Equal(HttpStatusCode.OK, redeemResponse.StatusCode);

        var redeemedVoucher =
            await redeemResponse.Content.ReadFromJsonAsync<VoucherResponse>();

        Assert.NotNull(redeemedVoucher);
        Assert.Equal(createdVoucher.Id, redeemedVoucher.Id);
        Assert.Equal("Redeemed", redeemedVoucher.Status);
    }


    [Fact]
    public async Task RedeemVoucher_WhenVoucherDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.PostAsync(
            "/api/vouchers/999/redeem",
            content: null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task RedeemVoucher_WhenAlreadyRedeemed_ReturnsConflict()
    {
        // Arrange: create a voucher
        var createResponse = await _client.PostAsJsonAsync(
            "/api/vouchers",
            new { Amount = 250m });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var voucher =
            await createResponse.Content.ReadFromJsonAsync<VoucherResponse>();

        Assert.NotNull(voucher);

        // Redeem it once
        var firstResponse = await _client.PostAsync(
            $"/api/vouchers/{voucher.Id}/redeem",
            content: null);

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        // Act: attempt to redeem it again
        var secondResponse = await _client.PostAsync(
            $"/api/vouchers/{voucher.Id}/redeem",
            content: null);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }


[Fact]
public async Task RedeemVoucher_WhenVoucherHasExpired_ReturnsConflict()
{
    // Arrange: add an already-expired voucher to the test database
    using var scope = _factory.Services.CreateScope();
    var dbContext = scope.ServiceProvider
        .GetRequiredService<VoucherHubDbContext>();

    var voucher = new Voucher
    {
        Code = "VH-EXPIRED123",
        Amount = 250m,
        Status = VoucherStatus.Active,
        ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1)
    };

    dbContext.Vouchers.Add(voucher);
    await dbContext.SaveChangesAsync();

    // Act: attempt to redeem the expired voucher
    var response = await _client.PostAsync(
        $"/api/vouchers/{voucher.Id}/redeem",
        content: null);

    // Assert
    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

    // Verify that its status was updated
    await dbContext.Entry(voucher).ReloadAsync();
    Assert.Equal(VoucherStatus.Expired, voucher.Status);
}



    [Fact]
    public async Task CancelVoucher_WhenActive_ReturnsCancelledVoucher()
    {
        // Arrange: create a voucher
        var createResponse = await _client.PostAsJsonAsync(
            "/api/vouchers",
            new { Amount = 250m });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var voucher =
            await createResponse.Content.ReadFromJsonAsync<VoucherResponse>();

        Assert.NotNull(voucher);

        // Act: cancel the voucher
        var cancelResponse = await _client.DeleteAsync(
            $"/api/vouchers/{voucher.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);

        var cancelledVoucher =
            await cancelResponse.Content.ReadFromJsonAsync<VoucherResponse>();

        Assert.NotNull(cancelledVoucher);
        Assert.Equal(voucher.Id, cancelledVoucher.Id);
        Assert.Equal("Cancelled", cancelledVoucher.Status);
    }


[Fact]
public async Task CancelVoucher_WhenVoucherDoesNotExist_ReturnsNotFound()
{
    var response = await _client.DeleteAsync("/api/vouchers/999");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
}


[Fact]
public async Task CancelVoucher_WhenAlreadyRedeemed_ReturnsConflict()
{
    // Arrange: create a voucher
    var createResponse = await _client.PostAsJsonAsync(
        "/api/vouchers",
        new { Amount = 250m });

    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

    var voucher = await createResponse.Content
        .ReadFromJsonAsync<VoucherResponse>();

    Assert.NotNull(voucher);

    // Redeem the voucher first
    var redeemResponse = await _client.PostAsync(
        $"/api/vouchers/{voucher.Id}/redeem",
        content: null);

    Assert.Equal(HttpStatusCode.OK, redeemResponse.StatusCode);

    // Act: attempt to cancel the redeemed voucher
    var cancelResponse = await _client.DeleteAsync(
        $"/api/vouchers/{voucher.Id}");

    // Assert
    Assert.Equal(HttpStatusCode.Conflict, cancelResponse.StatusCode);
}



    private class VoucherResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
