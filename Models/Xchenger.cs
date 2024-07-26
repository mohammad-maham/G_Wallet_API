using System;
using System.Collections.Generic;
using NodaTime;

namespace G_Wallet_API.Models;

public partial class Xchenger
{
    public long Id { get; set; }
    public long WalleId { get; set; }

    public string SourceAddress { get; set; } = null!;

    public long SourceWalletCurrency { get; set; }
    public decimal SourceAmount { get; set; }
    public string DestinationAddress { get; set; } = null!;

    public long DestinationWalletCurrency { get; set; }

    public decimal DestinationAmount { get; set; }

    public string? XchengData { get; set; } = null!;
}
