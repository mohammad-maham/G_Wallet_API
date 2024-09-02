using G_Wallet_API.Models;
using G_Wallet_API.Models.VM;

namespace G_Wallet_API.BusinessLogic.Interfaces;

public interface IFund
{
    //WalletCurrency? Deposit(TransactionVM model);
    TransactionVM AddTransaction(TransactionVM model);
    WalletCurrency? ConfirmTransaction(TransactionVM model);
    Wallet? GetWallet(int userId);
    WalletBankAccount? AddBankAccount(WalletBankAccount model);
    WalletCurrency AddExchange(Xchenger model);
    IEnumerable<WalletCurrencyVM> GetWalletCurrency(int userId);

    WalletCurrency? FindWallerCurrency(long walletId, long currencyId);
    IEnumerable<WalletBankAccountVM> GetBankAccounts(int walletId);

    WalletBankAccount? ToggleBankCard(WalletBankAccount model);
    IEnumerable<ReportVM> GetFinancialReport(FilterVM model);
    IEnumerable<ReportVM> GetExchanges(FilterVM model);
    IEnumerable<ReportVM?> GetTransaction(FilterVM model);

}
