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

    public async Task<WalletCurrency?> FindWallerCurrencyAsync(long walletId, int currencyId)
    {
        var wc = await _wallet.WalletCurrencies.FirstOrDefaultAsync(x => x.WalletId == walletId && x.CurrencyId == currencyId);
        return wc;
    }

    public async Task<Wallet?> GetWallet(int userId)
    {
        var t = await _wallet.Wallets.FirstOrDefaultAsync(x => x.UserId == userId);
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

            await _wallet.Wallets.AddAsync(w);

            var wc = new WalletCurrency
            {
                Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_walletcurrency"),
                WalletId = walletId,
                Amount = 0,
                CurrencyId = 1,
                RegDate = DateTime.Now,
                Status = 1,
                WcAddress = GetWcAdress(),
            };
            await _wallet.WalletCurrencies.AddAsync(wc);

            await _wallet.SaveChangesAsync();

            return w;
        }

        return t;
    }


    public async Task<IEnumerable<WalletCurrencyVM>> GetWalletCurrency(int userId)
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

        var res = dt.AsEnumerable<WalletCurrencyVM>();

        //var res = await _wallet.Wallets
        //    .SelectMany(x =>
        //    _wallet.WalletCurrencies
        //    .Where(y => y.WalletId == x.Id), (a, b) => new { a, b })
        //    .Select(x => x.b).ToListAsync();

        return res;
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

    public async Task<WalletBankAccount?> Deposit(TransactionVM model)
    {
        var t = new Transaction
        {
            Id= DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_transactionid"),
            OrderId=model.OrderId,
            Info=model.Info,
            TrackingCode=model.TrackingCode,
            TransactionDate=DateTime.Now,
            TransactionModeId= (short)Enums.TransactionMode.Online,
            WalletCurrencyId=model.WalletCurrencyId,
            WalletId=model.WalletId,
            Status=1,
            TransactionTypeId=(short)Enums.TransactionType.Deposit
        }

        if (t == null)
        {
            var tr = new TransactionVM();
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

    public async Task<WalletCurrency> AddExchange(Xchenger model)
    {

        var sourceWallet = await FindWallerCurrencyAsync(model.WalletId, model.SourceWalletCurrency);
        var destWallet = await FindWallerCurrencyAsync(model.WalletId, model.DestinationWalletCurrency);

        using (var context = new GWalletDbContext())
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    _ = await _wallet.Xchengers.AddAsync(model);

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
                            WcAddress= GetWcAdress()
                        };

                        await _wallet.WalletCurrencies.AddAsync(destWallet);

                    }

                    await _wallet.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return destWallet!;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }

            }

        }


    }

    private static string GetWcAdress()
    {
        byte[] buffer = Guid.NewGuid().ToByteArray();
        buffer[0] = 0;
        buffer[1] = 0;
        buffer[2] = 0;
        buffer[3] = 0;

        return new Guid(buffer).ToString();
    }
}




