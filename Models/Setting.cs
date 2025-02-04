using System;
using System.Collections.Generic;

namespace G_Wallet_API.Models;

public partial class Setting
{
    public int Id { get; set; }

    public string Nsme { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string Caption { get; set; } = null!;

    public string? Description { get; set; }
}
