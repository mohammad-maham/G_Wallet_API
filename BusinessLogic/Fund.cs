using G_Wallet_API.BusinessLogic.Interfaces;
using G_Wallet_API.Common;
using G_Wallet_API.Models;
using G_Wallet_API.Models.VM;
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

    public WalletCurrency? FindWallerCurrency(long walletId, long currencyId)
    {
        var wc = _wallet.WalletCurrencies.FirstOrDefault(x => x.WalletId == walletId && x.CurrencyId == currencyId);
        return wc;
    }

    public Wallet? GetWallet(int userId)
    {
        var t = _wallet.Wallets.FirstOrDefault(x => x.UserId == userId);
        long walletId = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_walletid");

        if (t == null)
        {
            Wallet? w = new()
            {
                Id = walletId,
                UserId = userId,
                CreateDate = DateTime.Now,
                Status = 1
            };

            _wallet.Wallets.Add(w);

            var wc = new WalletCurrency
            {
                Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_walletcurrency"),
                WalletId = walletId,
                Amount = 0,
                CurrencyId = 1,
                RegDate = DateTime.Now,
                Status = 1,
                WcAddress = Guid.NewGuid().ToString(),
            };
            _wallet.WalletCurrencies.Add(wc);

            _wallet.SaveChanges();

            return w;
        }

        return t;
    }


    public IEnumerable<WalletCurrencyVM> GetWalletCurrency(int userId)
    {

        var query = @$"SELECT WC.*,
	                    CU.""Name""  AS ""CurrencyName"",
	                    U.""Name"" AS ""Unit""
                    FROM ""WalletCurrency"" WC,
	                    ""Wallet"" W,
	                    ""Currency"" CU,
	                    ""Unit"" U
                    WHERE W.""UserId"" = {userId}
	                    AND WC.""WalletId"" = W.""Id""
	                    AND WC.""CurrencyId"" = CU.""Id""
	                    AND CU.""UnitId"" = U.""Id""";

        var dt = new PostgresDbHelper().RunQuery(query);

        var res = dt.AsEnumerable<WalletCurrencyVM>();

        //var res = await _wallet.Wallets
        //    .SelectMany(x =>
        //    _wallet.WalletCurrencies
        //    .Where(y => y.WalletId == x.Id), (a, b) => new { a, b })
        //    .Select(x => x.b).ToListAsync();

        return res;
    }


    public WalletBankAccount? AddBankAccount(WalletBankAccount model)
    {
        var t = _wallet.WalletBankAccounts.FirstOrDefault(x => x.Id == model.Id);

        if (t == null)
        {
            var w = new WalletBankAccount();
            w = model;
            
            w.Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_bankaccount");
            w.RegDate= DateTime.Now;

            _wallet.WalletBankAccounts.Add(model);
            _wallet.SaveChanges();

            return w;
        }

        _wallet.Entry(t).State = EntityState.Modified;
        _wallet.SaveChanges();

        return t;
    }


    public Transaction? AddTransaction(TransactionVM model)
    {

        var t = new Transaction
        {
            Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_transactionid"),
            OrderId = model.OrderId,
            Info = model.Info,
            TrackingCode = model.TrackingCode,
            TransactionDate = DateTime.Now,
            TransactionModeId = (short)Enums.TransactionMode.Online,
            WalletCurrencyId = model.WalletCurrencyId,
            WalletId = model.WalletId,
            Status = 1,
            TransactionTypeId = (short)Enums.TransactionType.Deposit
        };
        _wallet.Transactions.Add(t);
        return t;
    }


    public WalletCurrency? Deposit(TransactionVM model)
    {
        var wallet = FindWallerCurrency(model.WalletId, model.WalletCurrencyId);

        using (var context = new GWalletDbContext())
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {

                    var t = new Transaction
                    {
                        Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_transactionid"),
                        OrderId = model.OrderId,
                        Info = model.Info,
                        TrackingCode = model.TrackingCode,
                        TransactionDate = DateTime.Now,
                        TransactionModeId = (short)Enums.TransactionMode.Online,
                        WalletCurrencyId = model.WalletCurrencyId,
                        WalletId = model.WalletId,
                        Status = 1,
                        TransactionTypeId = (short)Enums.TransactionType.Deposit
                    };
                    _wallet.Transactions.Add(t);

                    wallet.Amount += model.Amount;
                    _wallet.WalletCurrencies.Update(wallet);

                    _wallet.SaveChanges();
                    transaction.Commit();

                    return wallet!;
                }
                catch (Exception)
                {
                    transaction.Commit();
                    throw;
                }

            }

        }

        return null;
    }

    public WalletCurrency? Windrow(TransactionVM model)
    {
        var wallet = FindWallerCurrency(model.WalletId, model.WalletCurrencyId);

        using (var context = new GWalletDbContext())
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {

                    var t = new Transaction
                    {
                        Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_transactionid"),
                        OrderId = model.OrderId,
                        Info = model.Info,
                        TrackingCode = model.TrackingCode,
                        TransactionDate = DateTime.Now,
                        TransactionModeId = (short)Enums.TransactionMode.Online,
                        WalletCurrencyId = model.WalletCurrencyId,
                        WalletId = model.WalletId,
                        Status = 1,
                        TransactionTypeId = (short)Enums.TransactionType.Deposit
                    };
                    _wallet.Transactions.Add(t);

                    wallet.Amount -= model.Amount;
                    _wallet.WalletCurrencies.Update(wallet);

                    _wallet.SaveChanges();
                    transaction.Commit();

                    return wallet!;
                }
                catch (Exception)
                {
                    transaction.Commit();
                    throw;
                }

            }

        }

        return null;
    }

    public WalletCurrency AddExchange(Xchenger model)
    {

        var sourceWallet = FindWallerCurrency(model.WalletId, model.SourceWalletCurrency);
        var destWallet = FindWallerCurrency(model.WalletId, model.DestinationWalletCurrency);

        using (var context = new GWalletDbContext())
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    model.Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_exchanger");

                    _ = _wallet.Xchengers.Add(model);

                    sourceWallet!.Amount -= model.SourceAmount;
                    _wallet.Update(sourceWallet);

                    if (destWallet != null)
                    {
                        destWallet.Amount += model.DestinationAmout;
                        _wallet.Update(destWallet);
                    }

                    if (destWallet == null)
                    {
                        //string unitId = string.Join(", ", _wallet.Currencies.Where(x => x.Id == model.DestinationWalletCurrency).Select(x => new { x.UnitId }));

                        destWallet = new WalletCurrency
                        {
                            Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_walletcurrency"),
                            WalletId = model.WalletId,
                            Amount = model.DestinationAmout,
                            CurrencyId = (short)model.DestinationWalletCurrency,
                            RegDate = DateTime.Now,
                            Status = 1,
                            WcAddress =  Guid.NewGuid().ToString()
                        };

                        _wallet.WalletCurrencies.Add(destWallet);

                    }

                    _wallet.SaveChanges();
                    transaction.Commit();

                    return destWallet!;
                }
                catch (Exception)
                {
                      transaction.Commit();
                    throw;
                }

            }

        }


    }

}




