using VoucherHub.Modules.Vouchers.Application.DTOs;
using VoucherHub.Modules.Vouchers.Application.Services;

namespace VoucherHub.Modules.Vouchers.API;

public static class VoucherEndpoints
{
    public static void MapVoucherEndpoints(this WebApplication app)
    {

        app.MapPost("/api/vouchers", async (
            CreateVoucherRequest request,
            VoucherService voucherService) =>
        {
            try
            {
                var voucher = await voucherService.CreateVoucherAsync(request);

                return Results.Created(
                    $"/api/vouchers/{voucher.Id}",
                    voucher);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });


        app.MapPut("/api/vouchers/{id}", async (
            int id,
            UpdateVoucherRequest request,
            VoucherService voucherService) =>
        {
            try
            {
                var voucher = await voucherService.UpdateVoucherAsync(id, request);

                if (voucher is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(voucher);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });

        app.MapGet("/api/vouchers", async (VoucherService voucherService) =>
        {
            var vouchers = await voucherService.GetAllVouchersAsync();

            return Results.Ok(vouchers);
        });

        app.MapGet("/api/vouchers/{id:int}", async (int id, VoucherService voucherService) =>
        {
            var voucher = await voucherService.GetVoucherByIdAsync(id);

            if (voucher is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(voucher);
        });

        app.MapDelete("/api/vouchers/{id}", async (int id, VoucherService voucherService) =>
        {
            try
            {
                var voucher = await voucherService.CancelVoucherAsync(id);

                if (voucher is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(voucher);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { message = ex.Message });
            }

        });

        app.MapPost("/api/vouchers/{id}/redeem", async (int id, VoucherService voucherService) =>
        {
            try
            {
                var voucher = await voucherService.RedeemVoucherAsync(id);

                if (voucher is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(voucher);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { message = ex.Message });
            }
        });
    }
}

