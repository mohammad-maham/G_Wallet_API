using System;
using System.Collections.Generic;
using NodaTime;

namespace G_Wallet_API.Models;

public partial class TransactionConfirmation
{
    public long Id { get; set; }

    public long TransactionId { get; set; }

    public short Status { get; set; }

    public long ConfirmationUserId { get; set; }

    public OffsetTime ConfirmationDate { get; set; }

    public string? RequestDescription { get; set; }

    public string? ResponceDescription { get; set; }

    public string? TransactionInfo { get; set; }
}
