using System;
using System.Collections.Generic;

namespace G_Wallet_API.Models;

public partial class TransactionDetail
{
    public long Id { get; set; }

    public long TransactionId { get; set; }

    public decimal EntityPrice { get; set; }

    public decimal CurentAmount { get; set; }

    public decimal ChangeFromLast { get; set; }
}
