using G_Wallet_API.Models;

namespace G_Wallet_API.BusinessLogic.Interfaces;

public interface IFund
{
    Task<WalletCurrency?> Deposit(WalletCurrency model);
    Task<WalletCurrency?> Sell(WalletCurrency model);
    Task<WalletCurrency?> Buy(WalletCurrency model);
    Task<WalletCurrency?> Windrow(WalletCurrency model);
    Task<Transaction?> AddTransaction(Transaction model);
    Task<Wallet?> GetWallet(Wallet model);
    Task<WalletBankAccount?> AddBankAccount(WalletBankAccount model);
    Task <IEnumerable<WalletCurrency>> AddExchange(Xchenger model);
     Task<IEnumerable<WalletCurrency>> GetWalletCurrency(Wallet model);

}
