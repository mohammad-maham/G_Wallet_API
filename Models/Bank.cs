using System;
using System.Collections.Generic;

namespace G_Wallet_API.Models;

public partial class Bank
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public short Status { get; set; }

    public string? LogoPath { get; set; }
}
