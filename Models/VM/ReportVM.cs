using System;
using System.Collections.Generic;
using NodaTime;

namespace G_Wallet_API.Models.VM;

public partial class ReportVM
{
    public long? Id { get; set; }
    public long? TransactionConfirmId { get; set; }

    public long? WalletId { get; set; }

    public short? TransactionTypeId { get; set; }
    public string? TransactionType { get; set; }

    public string? TransactionMode { get; set; }
    public short? TransactionModeId { get; set; }

    public short? Status { get; set; }

    public string? RepDate { get; set; }

    public string? Info { get; set; }

    public string? OrderId { get; set; }

    public string? TrackingCode { get; set; }

    public decimal? SourceAmount { get; set; }
    public decimal? DestinationAmout { get; set; }

    public string? SourceAddress { get; set; }

    public string? DestinationAddress { get; set; }

    public long? SourceWalletCurrencyId { get; set; }
    public string? SourceWalletCurrency { get; set; }

    public long? DestinationWalletCurrencyId { get; set; }
    public string? DestinationWalletCurrency { get; set; }

    public long? UserId { get; set; }


    public long? ConfirmationUserId { get; set; }

    public string? ConfirmationDate { get; set; }

    public string? RequestDescription { get; set; }

    public string? ResponceDescription { get; set; }

    public string? TransactionInfo { get; set; }

}
