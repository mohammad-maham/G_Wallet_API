using G_Wallet_API.Models;
using RestSharp.Authenticators;
using G_Wallet_API.BusinessLogic.Interfaces;

namespace G_Wallet_API.BusinessLogic;

public class Func : IFund
{
    private readonly ILogger<Func>? _logger;
    private readonly GWalletDbContext _wallet;

    public Func(GWalletDbContext wallet, ILogger<Func> logger)
    {
        _wallet = wallet;
        _logger = logger;
    }
    public void Deposit(int userId, decimal amount)
    {

        //return await _wallet.Wallets.se(x =>
        //(x.UserName == username && isUsername) ||
        //(x.NationalCode == long.Parse(username) && !isUsername)); ;
    }

   
}