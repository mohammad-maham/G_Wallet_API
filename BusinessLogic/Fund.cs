using G_Wallet_API.BusinessLogic.Interfaces;
using G_Wallet_API.Common;
using G_Wallet_API.Models;
using G_Wallet_API.Models.VM;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;

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

    public IEnumerable<WalletBankAccountVM> GetBankAccounts(int walletId)
    {

        var query = @$"SELECT *
                    FROM ""WalletBankAccount"" BA,
	                    ""Bank"" B
                    WHERE BA.""WalletId"" = {walletId}
	                    AND BA.""BankId"" = B.""Id""";

        var dt = new PostgresDbHelper().RunQuery(query);

        var res = dt.AsEnumerable<WalletBankAccountVM>();

        return res;
    }

    public WalletBankAccount? AddBankAccount(WalletBankAccount model)
    {
        var w = new WalletBankAccount
        {
            Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_bankaccount1"),
            BankId = model.BankId,
            BankAccountNumber = model.BankAccountNumber,
            Status = 1,
            OrderId = model.OrderId,
            RegDate = DateTime.Now,
            RegionId = model.RegionId,
            Shaba = model.Shaba,
            WalletId = model.WalletId,
            ValidationInfo = model.ValidationInfo,
            Name = model.Name,
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

    public TransactionVM AddTransaction(TransactionVM model)
    {
        var tranactionId = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_transactionid");
        var t = new Transaction
        {
            Id = tranactionId,
            //OrderId = model.OrderId,
            //Info = JsonConvert.SerializeObject(model.Info),
            TrackingCode = model.TrackingCode,
            TransactionDate = DateTime.Now,
            TransactionModeId = (short)model.TransactionModeId,
            WalletCurrencyId = model.WalletCurrencyId,
            WalletId = model.WalletId,
            Status = 1,
            TransactionTypeId = (short)model.TransactionTypeId,
            Amount = model.Amount
        };
        _wallet.Transactions.Add(t);

        var transConfirmId = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_transconfirm");
        var trc = new TransactionConfirmation
        {
            Id = transConfirmId,
            //ConfirmationDate = DateTime.Now,
            TransactionId = tranactionId,
            RequestDescription = model.RequestDescription,
            ResponceDescription = model.ResponceDescription,
            Status = 0,
            TransactionInfo = JsonConvert.SerializeObject(model.TransactionInfo)
        };
        _wallet.TransactionConfirmations.Add(trc);

        _wallet.SaveChanges();

        var tvm = new TransactionVM
        {
            Id = tranactionId,
            TransactionConfirmId = transConfirmId,
            Amount = t.Amount,
            WalletId = t.WalletId,
            WalletCurrencyId = t.WalletCurrencyId,
            Status = trc.Status,
        };
        return tvm;
    }

    //public WalletCurrency? Deposit(TransactionVM model)
    //{
    //    var wallet = FindWallerCurrency(model.WalletId, model.WalletCurrencyId);

    //    using (var context = new GWalletDbContext())
    //    {
    //        using (var transaction = context.Database.BeginTransaction())
    //        {
    //            try
    //            {

    //                var t = new Transaction
    //                {
    //                    Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_transactionid"),
    //                    OrderId = model.OrderId,
    //                    Info = JsonConvert.SerializeObject(model.Info),
    //                    TrackingCode = model.TrackingCode,
    //                    TransactionDate = DateTime.Now,
    //                    TransactionModeId = (short)Enums.TransactionMode.Online,
    //                    WalletCurrencyId = model.WalletCurrencyId,
    //                    WalletId = model.WalletId,
    //                    Status = 1,
    //                    TransactionTypeId = (short)Enums.TransactionType.Deposit,

    //                };
    //                _wallet.Transactions.Add(t);

    //                wallet!.Amount += model.Amount;
    //                _wallet.WalletCurrencies.Update(wallet);

    //                _wallet.SaveChanges();
    //                transaction.Commit();

    //                return wallet!;
    //            }
    //            catch (Exception)
    //            {
    //                transaction.Commit();
    //                throw;
    //            }

    //        }

    //    }

    //}

    public WalletCurrency? ConfirmTransaction(TransactionVM model)
    {
        var wallet = FindWallerCurrency(model.WalletId, model.WalletCurrencyId);

        using (var context = new GWalletDbContext())
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var trc = _wallet.TransactionConfirmations.FirstOrDefault(x => x.Id == model.TransactionConfirmId);
                    trc.Status = 1;
                    trc.ConfirmationDate = DateTime.Now;
                    trc.ResponceDescription = model.ResponceDescription;
                    trc.ConfirmationUserId = model.ConfirmationUserId;
                    trc.TransactionInfo = model.TransactionInfo;
                    _wallet.TransactionConfirmations.Update(trc);

                    wallet.Amount += model.Amount;
                    _wallet.WalletCurrencies.Update(wallet);

                    _wallet.SaveChanges();
                    transaction.Commit();

                    return wallet!;
                }
                catch (Exception)
                {
                    transaction.Dispose();
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
                    transaction.Dispose();
                    throw;
                }

            }

        }

    }

    public IEnumerable<ReportVM> GetTransaction(FilterVM model)
    {
        var transactions = _wallet.Transactions.ToList();

        if (model.UserId != null)
        {
            var w = _wallet.Wallets.FirstOrDefault(x => x.UserId == model.UserId);
            transactions = transactions.Where(x => x.WalletId == w.Id).ToList();
        }

        if (model.TransactionTypeId != null)
            transactions = transactions.Where(x => x.TransactionTypeId == model.TransactionTypeId).ToList();

        if (model.TransactionModeId != null)
            transactions = transactions.Where(x => x.TransactionModeId == model.TransactionModeId).ToList();

        if (model.FromDate != null)
            transactions = transactions.Where(x => x.TransactionDate >= model.FromDate).ToList();

        if (model.ToDate != null)
            transactions = transactions.Where(x => x.TransactionDate <= model.ToDate).ToList();

        var t = transactions
            .SelectMany(tr => _wallet.TransactionConfirmations.Where(tf => tf != null && tf.TransactionId == tr.Id)
            .DefaultIfEmpty(), (tr, tf) => new { tr, tf })
            .ToList();

        var res = t.Select(x => new ReportVM()
        {
            Id = x.tr.Id,
            SourceAmount = x.tr.Amount != null ? x.tr.Amount : 0,
            RepDate = ConvertToPersianDate(x.tr.TransactionDate),
            WalletId = x.tr.WalletId,
            SourceWalletCurrencyId = x.tr.WalletCurrencyId,
            TransactionTypeId = x.tr.TransactionTypeId,
            TransactionModeId = x.tr.TransactionModeId,
            SourceWalletCurrency = GetCurrencyType(x.tr.WalletCurrencyId),
            TransactionType = GetTransactionType(x.tr.TransactionTypeId),
            TransactionMode = GetTransactionMode(x.tr.TransactionModeId),
            ConfirmationUserId = x.tf?.ConfirmationUserId ?? -1,
            TransactionConfirmId = x.tf?.Id ?? -1,
            RequestDescription = x.tf?.RequestDescription ?? "",
            ResponceDescription = x.tf?.ResponceDescription ?? "",
            TransactionInfo = x.tf?.TransactionInfo ?? "",
            Status = x.tf?.Status ?? 0,
            ConfirmationDate = ConvertToPersianDate(x.tf?.ConfirmationDate ?? null),
            UserId = model.UserId
        }).ToList();

        return res;
    }

    public IEnumerable<ReportVM> GetExchanges(FilterVM model)
    {

        var t = _wallet.Xchengers.Where(x => x.WalletId == model.WalletId).ToList();

        if (model.FromDate != null)
            t = t.Where(x => x.ExChangeData >= model.FromDate).ToList();

        if (model.ToDate != null)
            t = t.Where(x => x.ExChangeData <= model.ToDate).ToList();

        var res = t.Select(x => new ReportVM()
        {
            Id = x.Id,
            SourceAmount = x.SourceAmount,
            DestinationAmout = x.DestinationAmout,
            RepDate = ConvertToPersianDate(x.ExChangeData),
            WalletId = x.WalletId,
            TransactionType = GetExchangeType(x.SourceWalletCurrency),
            SourceWalletCurrencyId = x.SourceWalletCurrency,
            DestinationWalletCurrencyId = x.DestinationWalletCurrency,
            UserId = x.RegUserId
        }).ToList();

        return res;
    }

    public IEnumerable<ReportVM> GetFinancialReport(FilterVM model)
    {

        var transactions = _wallet.Transactions.ToList();
        var xchanges = _wallet.Xchengers.ToList();

        if (model.UserId != null)
        {
            var w = _wallet.Wallets.FirstOrDefault(x => x.UserId == model.UserId);

            transactions = transactions.Where(x => x.WalletId == w.Id).ToList();
            xchanges = xchanges.Where(x => x.WalletId == w.Id).ToList();
        }

        if (model.TransactionTypeId != null)
            transactions = transactions.Where(x => x.TransactionTypeId == model.TransactionTypeId).ToList();

        if (model.TransactionModeId != null)
            transactions = transactions.Where(x => x.TransactionModeId == model.TransactionModeId).ToList();

        if (model.FromDate != null)
        {
            transactions = transactions.Where(x => x.TransactionDate >= model.FromDate).ToList();
            xchanges = xchanges.Where(x => x.ExChangeData <= model.FromDate).ToList();
        }

        if (model.ToDate != null)
        {
            transactions = transactions.Where(x => x.TransactionDate <= model.ToDate).ToList();
            xchanges = xchanges.Where(x => x.ExChangeData <= model.ToDate).ToList();
        }

        var t = transactions
            .SelectMany(tr => _wallet.TransactionConfirmations.Where(tf => tf.TransactionId == tr.Id && tf.Status == 1)
            .DefaultIfEmpty(), (tr, tf) => new { tr, tf })
            .ToList();

        var rest = t.Select(x => new ReportVM()
        {
            Id = x.tr.Id,
            SourceAmount = x.tr.Amount != null ? x.tr.Amount : 0,
            DestinationAmout = null,
            RepDate = ConvertToPersianDate(x.tr.TransactionDate),
            WalletId = x.tr.WalletId,
            SourceWalletCurrencyId = x.tr.WalletCurrencyId,
            TransactionTypeId = x.tr.TransactionTypeId,
            TransactionModeId = x.tr.TransactionModeId,
            SourceWalletCurrency = GetCurrencyType(x.tr.WalletCurrencyId),
            DestinationWalletCurrencyId = null,
            TransactionType = GetTransactionType(x.tr.TransactionTypeId),
            TransactionMode = GetTransactionMode(x.tr.TransactionModeId),
            ConfirmationUserId = x.tf?.ConfirmationUserId ?? -1,
            TransactionConfirmId = x.tf?.Id ?? -1,
            RequestDescription = x.tf?.RequestDescription ?? "",
            ResponceDescription = x.tf?.ResponceDescription ?? "",
            TransactionInfo = x.tf?.TransactionInfo ?? "",
            Status = x.tf?.Status ?? 0,
            ConfirmationDate = ConvertToPersianDate(x.tf?.ConfirmationDate ?? null),
        }).ToList();


        var resx = xchanges.Select(x => new ReportVM
        {
            Id = x.Id,
            SourceAmount = x.SourceAmount,
            DestinationAmout = x.DestinationAmout,
            RepDate = ConvertToPersianDate(x.ExChangeData),
            WalletId = x.WalletId,
            SourceWalletCurrencyId = x.SourceWalletCurrency,
            DestinationWalletCurrencyId = x.DestinationWalletCurrency,
            TransactionTypeId = (short)Enums.TransactionType.Exchange,
            TransactionModeId = (short)Enums.TransactionMode.Online,
            SourceWalletCurrency = GetCurrencyType(x.SourceWalletCurrency),
            TransactionType = GetExchangeType(x.SourceWalletCurrency),
            TransactionMode = GetTransactionMode((short)Enums.TransactionMode.Online),
            ConfirmationUserId = null,
            TransactionConfirmId = null,
            RequestDescription = null,
            ResponceDescription = null,
            TransactionInfo = null,
            Status = 1,
            ConfirmationDate = null,
        }).ToList();

        var res = rest.Union(resx).Distinct();
        return res;
    }

    private string ConvertToPersianDate(DateTime? date)
    {
        try
        {
            string persianDateString = date.GetValueOrDefault().ToString("yyyy/MM/dd HH:mm:ss", new CultureInfo("fa-IR"));
            return persianDateString;
        }
        catch (Exception)
        {

            return "";
        }

    }

    public string GetTransactionType(long tr)
    {
        return tr switch
        {
            1 => "فروش",
            2 => "خرید",
            3 => "برداشت",
            4 => "واریز",
            5 => "تبدیل",
        };
    }

    public string GetTransactionMode(long tm)
    {
        return tm switch
        {
            1 => "آنلاین",
            2 => "آفلاین",
        };
    }

    public string GetCurrencyType(long cu)
    {
        return cu switch
        {
            1 => "پول",
            2 => "طلا",
        };
    }

    public string GetExchangeType(long wc)
    {
        return wc switch
        {
            1 => "پول به طلا",
            2 => "طلا به پول",
        };
    }
}




