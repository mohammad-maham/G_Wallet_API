using System;
using System.Collections.Generic;
using NodaTime;

namespace G_Wallet_API.Models;

public partial class WalletBankAccount
{
    public long Id { get; set; }

    public long WalletId { get; set; }

    public int BankId { get; set; }

    public decimal BankAccountNumber { get; set; }

    public int RegionId { get; set; }

    public short Status { get; set; }

    public string? RegDate { get; set; }

    public string Shaba { get; set; } = null!;

    public short OrderId { get; set; }

    public string? ValidationInfo { get; set; }
}
