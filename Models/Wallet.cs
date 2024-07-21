using System;
using System.Collections.Generic;

namespace G_Wallet_API.Models;

public partial class Wallet
{
    public long Id { get; set; }

    public long UserId { get; set; }


    public short Status { get; set; }
}
