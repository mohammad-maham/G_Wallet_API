namespace G_Wallet_API.Models.VM;

public class FilterVM
{

    public int? UserId { get; set; }
    public DateTime? ToDate { get; set; }
    public DateTime? FromDate { get; set; }
    public long? WalletId { get; set; }
    public long? WalletCurrencyId { get; set; }
    public short? TransactionTypeId { get; set; }
    public short? TransactionModeId { get; set; }
    public short? CurrencyId { get; set; }

}
