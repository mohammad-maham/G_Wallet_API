using Wallet.BusinessLogic.Interfaces;

namespace Wallet.BusinessLogic;

public class WalletService : IWalletService
{
    private readonly Dictionary<int, decimal> _wallets = new Dictionary<int, decimal>();

    public void Deposit(int userId, decimal amount)
    {
        if (!_wallets.ContainsKey(userId))
        {
            _wallets[userId] = 0;
        }
        _wallets[userId] += amount;
    }

    public void Withdraw(int userId, decimal amount)
    {
        if (!_wallets.ContainsKey(userId) || _wallets[userId] < amount)
        {
            throw new InvalidOperationException("Insufficient funds.");
        }
        _wallets[userId] -= amount;
    }

    public decimal GetBalance(int userId)
    {
        if (!_wallets.ContainsKey(userId))
        {
            return 0;
        }
        return _wallets[userId];
    }
}