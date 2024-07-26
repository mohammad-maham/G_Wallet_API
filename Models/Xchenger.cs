using System;
using System.Collections.Generic;
using NodaTime;

namespace G_Wallet_API.Models;

public partial class Xchenger
{
    public long Id { get; set; }

    public string SourceAddress { get; set; } = null!;

    public string DestinationAddress { get; set; } = null!;

    public long SourceWalletCurrency { get; set; }

    public long DestinationWalletCurrency { get; set; }

    public decimal Amount { get; set; }

    public string? XchengData { get; set; } = null!;
}
