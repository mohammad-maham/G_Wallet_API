using FarsiLibrary.Utils;
using G_Wallet_API.BusinessLogic.Interfaces;
using G_Wallet_API.Common;
using G_Wallet_API.Models;
using Microsoft.EntityFrameworkCore;

namespace G_Wallet_API.BusinessLogic;

public class Fund : IFund
{
    private readonly ILogger<Fund>? _logger;
    private readonly GWalletDbContext _wallet;

    public Fund(GWalletDbContext wallet, ILogger<Fund> logger)
    {
        _wallet = wallet;
        _logger = logger;
    }

    public async Task<Wallet?> GetWallet(int userId)
    {
        var t = await _wallet.Wallets.FirstOrDefaultAsync(x => x.UserId == userId);

        if (t == null)
        {
            Wallet? w = new()
            {
                Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_walletid"),
                UserId = userId,
                CreateDate = new PersianDate(DateTime.Now),
                Status = 1
            };

            await _wallet.Wallets.AddAsync(w);
            await _wallet.SaveChangesAsync();

            return w;
        }

        return t;
    }

    public async Task<IEnumerable<WalletCurrency>> GetWalletCurrency(int userId)
    {

        var query = @$"SELECT WC.*,
	                    CU.""Name"",
	                    U.""Name"" AS ""CurrencyName""
                    FROM ""WalletCurrency"" WC,
	                    ""Wallet"" W,
	                    ""Currency"" CU,
	                    ""Unit"" U
                    WHERE W.""UserId"" = {userId}
	                    AND WC.""WalletId"" = W.""Id""
	                    AND WC.""CurrencyId"" = CU.""Id""
	                    AND CU.""UnitId"" = U.""Id""";

        var dt = await new PostgresDbHelper().RunQuery(query);

        var res = dt.AsEnumerable<WalletCurrency>();

        //var res = await _wallet.Wallets
        //    .SelectMany(x =>
        //    _wallet.WalletCurrencies
        //    .Where(y => y.WalletId == x.Id), (a, b) => new { a, b })
        //    .Select(x => x.b).ToListAsync();

        return res;
    }

    public async Task<WalletCurrency?> Deposit(WalletCurrency model)
    {
        //model.tra = (int)Enums.TransactionType.Deposit;
        //model.TransactionModeId = (int)Enums.TransactionMode.Online;

        return await AddWalletCurrency(model);
    }

    public async Task<WalletCurrency?> Windrow(WalletCurrency model)
    {
        if (model.Amount > 0)
            model.Amount *= -1;

        //model.TransactionTypeId = (int)Enums.TransactionType.Windrow;
        //model.TransactionModeId = (int)Enums.TransactionMode.Online;

        return await AddWalletCurrency(model);
    }

    public async Task<WalletCurrency?> Sell(WalletCurrency model)
    {
        //model.TransactionTypeId = (int)Enums.TransactionType.Sell;
        //model.TransactionModeId = (int)Enums.TransactionMode.Online;

        return await AddWalletCurrency(model);
    }

    public async Task<WalletCurrency?> Buy(WalletCurrency model)
    {
        if (model.Amount > 0)
            model.Amount *= -1;

        //model.TransactionTypeId = (int)Enums.TransactionType.Buy;
        //model.TransactionModeId = (int)Enums.TransactionMode.Online;

        return await AddWalletCurrency(model);
    }

    public async Task<Transaction?> AddTransaction(Transaction model)
    {
        var t = await _wallet.Transactions.FirstOrDefaultAsync(x => x.WalletId == model.WalletId);
        //Transaction? w = new();

        if (t != null)
        {
            //w = new Transaction();
            //{
            //    Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_transactionid"),
            //    Info = model.Info,
            //    OrderId = model.OrderId,
            //    TrackingCode = model.TrackingCode,
            //    TransactionDate = model.TransactionDate,
            //    TransactionModeId = model.TransactionModeId,
            //    TransactionTypeId = model.TransactionTypeId,
            //    WalletCurrencyId = model.WalletCurrencyId,
            //    Status = model.Status
            //};
            await _wallet.Transactions.AddAsync(model);
            await _wallet.SaveChangesAsync();

            return model;
        }

        return null;
    }

    public async Task<WalletBankAccount?> AddBankAccount(WalletBankAccount model)
    {
        var t = await _wallet.WalletBankAccounts.FirstOrDefaultAsync(x => x.Id == model.Id);

        if (t == null)
        {
            var w = new WalletBankAccount();
            w = model;
            w.Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_bankaccount");

            await _wallet.WalletBankAccounts.AddAsync(w);
            await _wallet.SaveChangesAsync();

            return w;
        }

        _wallet.Entry(t).State = EntityState.Modified;
        await _wallet.SaveChangesAsync();

        return t;
    }

    private async Task<WalletCurrency?> AddWalletCurrency(WalletCurrency model)
    {

        using (var context = new GWalletDbContext())
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                await _wallet.Xchengers.AddAsync(new Xchenger
                {
                    Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_exchanger"),
                    ExChangeData = new PersianDate(DateTime.Now),
                    WalletId = model.WalletId,
                    RegUserId = model.user,
                    Status = 1,
                    SourceWalletCurrency = model.w
                });


                var t = await _wallet.WalletCurrencies
                    .Where(x => x.WalletId == model.WalletId && x.CurrencyId == model.CurrencyId)
                    .Select(x => new
                    {
                        x.Id,
                        x.WalletId,
                        x.CurrencyId,
                        x.Amount
                    })
                    .FirstOrDefaultAsync();


                if (t == null)
                {
                    var w = new WalletCurrency();
                    w = model;
                    w.Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_walletcurrency");

                    await _wallet.WalletCurrencies.AddAsync(w);
                    await _wallet.SaveChangesAsync();

                    return w;
                }

                t.Amount = t.Amount + model.Amount;

                _wallet.Entry(t).State = EntityState.Modified;
                await _wallet.SaveChangesAsync();

                return t;

            }
        }
    }

    public async Task<IEnumerable<WalletCurrency>> AddExchange(Xchenger model)
    {

        using (var context = new GWalletDbContext())
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                //model.ExChangeData = new PersianDate( model.ExChangeData.ToShortDateString());

                _ = await _wallet.Xchengers.AddAsync(model);

                var sourceWallet = new WalletCurrency
                {
                    WalletId = model.WalletId,
                    CurrencyId = model.SourceWalletCurrency,
                    Amount = -model.SourceAmount
                };

                 var t = await _wallet.WalletCurrencies
                    .Where(x => x.WalletId == model.WalletId && x.CurrencyId == model.DestinationWalletCurrency)
                    .Select(x => new
                    {
                        x.Id,
                        x.Amount += model.DestinationAmout
                    })
                    .FirstOrDefaultAsync();

                //WalletCurrency destWallet;
                if (t == null)
                {
                    t = new WalletCurrency
                    {
                        Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_walletcurrency"),
                        WalletId = model.WalletId,
                        Amount = model.DestinationAmout,
                        CurrencyId = model.DestinationWalletCurrency,
                        RegDate = DateTime.Now,
                        Status = 1,
                        Unit = _wallet.Currencies.Where(x => x.Id == model.DestinationWalletCurrency).Select(x => new { x.UnitId }),
                    };

                      await _wallet.WalletCurrencies.AddAsync(t);

                }

                if (t != null)
                    await _wallet.Update(t);
            }


            _wallet.Entry(t).State = EntityState.Modified;
            await _wallet.SaveChangesAsync();

            return t;

        }
        return null;
    }

}




