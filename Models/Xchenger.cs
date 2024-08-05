using System;
using System.Collections.Generic;
using NodaTime;

namespace G_Wallet_API.Models;

public partial class Xchenger
{
    public long Id { get; set; }

    public string? SourceAddress { get; set; } = null!;

    public string? DestinationAddress { get; set; } = null!;

    public short SourceWalletCurrency { get; set; }

    public long DestinationWalletCurrency { get; set; }

    public decimal SourceAmount { get; set; }

    public DateTime ExChangeData { get; set; }

    public long RegUserId { get; set; }

    public short Status { get; set; }

    public decimal DestinationAmout { get; set; }

    public long WalletId { get; set; }
}
