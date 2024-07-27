using FarsiLibrary.Utils;
using G_APIs.Common;
using G_Wallet_API.BusinessLogic.Interfaces;
using G_Wallet_API.Common;
using G_Wallet_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

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

    public async Task<Wallet?> GetWallet(Wallet model)
    {
        var t = await _wallet.Wallets.FirstOrDefaultAsync(x => x.UserId == model.UserId);

        if (t == null)
        {
            Wallet? w = new()
            {
                Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "SEQ_WalletId"),
                UserId = model.UserId,
                CreateDate = new PersianDate(DateTime.Now).ToString(),
                Status = 1
            };

            await _wallet.Wallets.AddAsync(w);
            await _wallet.SaveChangesAsync();

            return w;
        }

        return t;
    }

    public IEnumerable<WalletCurrency> GetWalletCurrency(WalletCurrency model)
    {
        var t = (from a in _wallet.WalletCurrencies where a.WalletId == model.WalletId select a).AsEnumerable<WalletCurrency>();
        return t;
    }

    public async Task<WalletCurrency?> Deposit(WalletCurrency model)
    {
        return await AddWalletCurrency(model);
    }

    public async Task<WalletCurrency?> Windrow(WalletCurrency model)
    {
        model.Amount *= -1;
        return await AddWalletCurrency(model);
    }

    public async Task<Transaction?> AddTransaction(Transaction model)
    {
        var t = await _wallet.Transactions.FirstOrDefaultAsync(x => x.WalletId == model.WalletId);
        Transaction? w = new();

        if (t != null)
        {
            w = new Transaction()
            {
                Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "SEQ_TransactionId"),
                Info = model.Info,
                OrderId = model.OrderId,
                TrackingCode = model.TrackingCode,
                TransactionDate = model.TransactionDate,
                TransactionModeId = model.TransactionModeId,
                TransactionTypeId = model.TransactionTypeId,
                WalletCurrencyId = model.WalletCurrencyId,
                Status = model.Status
            };
            await _wallet.Transactions.AddAsync(w);
            await _wallet.SaveChangesAsync();

            return w;
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
            w.Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "SEQ_bankAccount");

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
        var t = await _wallet.WalletCurrencies.FirstOrDefaultAsync(x => x.WalletId == model.WalletId && x.CurrencyId == model.CurrencyId);

        if (t == null)
        {
            var w = new WalletCurrency();
            w = model;
            w.Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "SEQ_walletCurrency");

            await _wallet.WalletCurrencies.AddAsync(w);
            await _wallet.SaveChangesAsync();

            return w;
        }

        t.Amount = t.Amount + model.Amount;

        _wallet.Entry(t).State = EntityState.Modified;
        await _wallet.SaveChangesAsync();

        return t;
    }

    public async Task<IEnumerable<WalletCurrency>> AddExchange(Xchenger model)
    {
        var sw = await Windrow(new WalletCurrency()
        {
            WalletId = model.WalleId,
            CurrencyId = model.SourceWalletCurrency,
            Amount = model.SourceAmount
        });

        if (sw != null)
        {
            var dw = await Deposit(new WalletCurrency()
            {
                WalletId = model.WalleId,
                CurrencyId = model.DestinationWalletCurrency,
                Amount = model.DestinationAmount
            });

            _wallet.SaveChanges();

            if (dw != null)
                return _wallet.WalletCurrencies.AsEnumerable<WalletCurrency>();
        }

        return null;
    }

}

 

