namespace G_Wallet_API.Models.VM
{
    public class FinancialVM
    {

            public long Id { get; set; }

            public long WalletId { get; set; }


            public short TransactionTypeId { get; set; }


            public DateTime TransactionDate { get; set; }


            public long SourceWalletCurrency { get; set; }

            public long DestinationWalletCurrency { get; set; }

            public decimal SourceAmount { get; set; }


            public decimal DestinationAmout { get; set; }


    }
}
