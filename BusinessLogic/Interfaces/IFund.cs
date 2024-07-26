using G_Wallet_API.Models;

namespace G_Wallet_API.BusinessLogic.Interfaces;

public interface IFund
{
    Task<WalletCurrency?> Deposit(WalletCurrency model);
    Task<WalletCurrency?> Windrow(WalletCurrency model);
    Task<Transaction?> AddTransaction(Transaction model);
    Task<Wallet?> GetWallet(Wallet model);
    //Task<WalletCurrency?> AddWalletCurrency(WalletCurrency model);
    Task<WalletBankAccount?> AddBankAccount(WalletBankAccount model);
    IEnumerable<WalletCurrency> GetWalletCurrency(WalletCurrency model);

}
