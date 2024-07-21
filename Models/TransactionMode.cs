using System;
using System.Collections.Generic;

namespace G_Wallet_API.Models;

public partial class TransactionMode
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
