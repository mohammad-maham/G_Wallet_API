using G_Wallet_API.BusinessLogic.Interfaces;
using G_Wallet_API.Common;
using G_Wallet_API.Models;
using G_Wallet_API.Models.VM;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

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

            var wcGold = new WalletCurrency();
            wcGold = wc;

            wcGold.Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_walletcurrency");
            wcGold.CurrencyId = 2;
           
            _wallet.WalletCurrencies.Add(wcGold);

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

    public DataTable GetBankAccounts(int walletId)
    {

        var query = @$"SELECT *
                    FROM ""WalletBankAccount"" BA,
	                    ""Bank"" B
                    WHERE BA.""WalletId"" = {walletId}
	                    AND BA.""BankId"" = B.""Id""";

        var dt = new PostgresDbHelper().RunQuery(query);

        //var res = dt.AsEnumerable<WalletBankAccountVM>();

        return dt;
    }

    public WalletBankAccount? AddBankAccount(WalletBankAccount model)
    {
        var w = new WalletBankAccount
        {
            Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_bankaccount1"),
            BankId = model.BankId,
            Name=model.Name,
            BankAccountNumber = model.BankAccountNumber,
            Status = 1,
            OrderId = model.OrderId,
            RegDate = DateTime.Now,
            RegionId = model.RegionId,
            Shaba = model.Shaba,
            WalletId = model.WalletId,
            ValidationInfo = model.ValidationInfo
        };

        _wallet.WalletBankAccounts.Add(w);
        _wallet.SaveChanges();

        return w;
    }

    public WalletBankAccount? ToggleBankCard(WalletBankAccount model)
    {
        try
        {
            var t = _wallet.WalletBankAccounts.FirstOrDefault(x => x.Id == model.Id);

            if (t != null)
            {
                t.Status = (short)(1 - t.Status)!;

                _wallet.WalletBankAccounts.Update(t);
                _wallet.SaveChanges();
                return t;
            }
            return null;
        }
        catch (Exception)
        {

            throw;
        }

    }

    public Transaction? AddTransaction(TransactionVM model)
    {
        var t = new Transaction
        {
            Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_transactionid"),
            OrderId = model.OrderId,
            Info = JsonConvert.SerializeObject(model.Info),
            TrackingCode = model.TrackingCode,
            TransactionDate = DateTime.Now,
            TransactionModeId = (short)Enums.TransactionMode.Online,
            WalletCurrencyId = model.WalletCurrencyId,
            WalletId = model.WalletId,
            Status = 1,
            TransactionTypeId = (short)Enums.TransactionType.Deposit
        };
        _wallet.Transactions.Add(t);
        _wallet.SaveChanges();
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
                        Info = JsonConvert.SerializeObject(model.Info),
                        TrackingCode = model.TrackingCode,
                        TransactionDate = DateTime.Now,
                        TransactionModeId = (short)Enums.TransactionMode.Online,
                        WalletCurrencyId = model.WalletCurrencyId,
                        WalletId = model.WalletId,
                        Status = 1,
                        TransactionTypeId = (short)Enums.TransactionType.Deposit,

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

    }

    public WalletCurrency? Windrow(WalletCurrency model)
    {
        var wallet = FindWallerCurrency(model.WalletId, model.CurrencyId);

        using (var context = new GWalletDbContext())
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {

                    var t = new Transaction
                    {
                        Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_transactionid"),
                        TransactionDate = DateTime.Now,
                        TransactionModeId = (short)Enums.TransactionMode.Online,
                        WalletCurrencyId = model.CurrencyId,
                        WalletId = model.WalletId,
                        Status = 1,
                        TransactionTypeId = (short)Enums.TransactionType.Windrow
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

    }

    public WalletCurrency AddExchange(Xchenger model)
    {

        var sourceWallet = FindWallerCurrency((long)model.WalletId, model.SourceWalletCurrency);
        var destWallet = FindWallerCurrency((long)model.WalletId, model.DestinationWalletCurrency);

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
                            WalletId = (long)model.WalletId,
                            Amount = model.DestinationAmout,
                            CurrencyId = (short)model.DestinationWalletCurrency,
                            RegDate = DateTime.Now,
                            Status = 1,
                            WcAddress = Guid.NewGuid().ToString()
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

    public List<Transaction?> GetTransaction(Wallet model)
    {
        var t = _wallet.Transactions.Where(x => x.WalletId == model.Id).ToList();
        return t;
    }
}




