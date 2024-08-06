using G_Wallet_API.Models;

namespace G_Wallet_API.BusinessLogic.Interfaces;

public interface IFund
{
    //Task<WalletCurrency?> Deposit(WalletCurrency model);
    //Task<WalletCurrency?> Windrow(WalletCurrency model);
    Task<Transaction?> AddTransaction(Transaction model);
    Task<Wallet?> GetWallet(int userId);
    Task<WalletBankAccount?> AddBankAccount(WalletBankAccount model);
    Task <WalletCurrency> AddExchange(Xchenger model);
     Task<IEnumerable<WalletCurrencyVM>> GetWalletCurrency(int userId);

    Task<WalletCurrency?> FindWallerCurrencyAsync(long walletId, int currencyId);


}
