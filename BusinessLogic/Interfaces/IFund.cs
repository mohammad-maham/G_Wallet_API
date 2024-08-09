using G_Wallet_API.Models;
using G_Wallet_API.Models.VM;

namespace G_Wallet_API.BusinessLogic.Interfaces;

public interface IFund
{
    WalletCurrency? Deposit(TransactionVM model);
    WalletCurrency? Windrow(WalletCurrency model);
    Transaction? AddTransaction(TransactionVM model);
    List<Transaction?> GetTransaction(Wallet model);
    Wallet? GetWallet(int userId);
    WalletBankAccount? AddBankAccount(WalletBankAccount model);
    WalletCurrency AddExchange(Xchenger model);
    IEnumerable<WalletCurrencyVM> GetWalletCurrency(int userId);

    WalletCurrency? FindWallerCurrency(long walletId, long currencyId);
    IEnumerable<WalletBankAccountVM> GetBankAccounts(int walletId);

    WalletBankAccount? ToggleBankCard(WalletBankAccount model);

}
