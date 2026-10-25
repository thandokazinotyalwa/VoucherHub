
using System.ComponentModel.DataAnnotations;

namespace VoucherHub.Modules.Vouchers.Application.DTOs;

public class UpdateVoucherRequest
{
    [Range(typeof(decimal), "0.01", "999999999.99",
        ParseLimitsInInvariantCulture = true,
        ErrorMessage = "Amount must be greater than zero and no more than 999999999.99.")]
    public decimal Amount { get; set; }
}
