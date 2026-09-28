namespace VoucherHub.Modules.Vouchers.Domain;

public class Voucher
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public VoucherStatus Status { get; set; } = VoucherStatus.Active;
}