namespace Wallet.BusinessLogic.Interfaces;

public interface IWalletService
{
    void Deposit(int userId, decimal amount);
    void Withdraw(int userId, decimal amount);
    decimal GetBalance(int userId);
}
