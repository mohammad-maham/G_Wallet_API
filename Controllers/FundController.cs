using G_Wallet_API.BusinessLogic.Interfaces;
using G_Wallet_API.Common;
using G_Wallet_API.Models;
using G_Wallet_API.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace G_Wallet_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FundController : ControllerBase
{
    private readonly IFund _fund;
    private readonly ILogger<FundController> _logger;
    private readonly GWalletDbContext _wallet;

    public FundController(IFund fund, ILogger<FundController> logger, GWalletDbContext wallet)
    {
        _fund = fund;
        _logger = logger;
        _wallet = wallet;
    }


    [HttpPost]
    [Route("[action]")]
    public IActionResult GetWallet([FromBody] Wallet model)
    {
        try
        {
            if (model.UserId <= 0)
                return BadRequest(new ApiResponse(500));

            var t = _fund.GetWallet((int)model.UserId);

            if (t != null)
            {
                string? jsonData = JsonConvert.SerializeObject(t);
                return Ok(new ApiResponse(data: jsonData));
            }

            return BadRequest(new ApiResponse(500));

        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            throw;

        }
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult GetWalletCurrency([FromBody] Wallet model)
    {
        try
        {

            var t = _fund.GetWalletCurrency((int)model.UserId);

            if (t == null)
                return BadRequest(new ApiResponse(400));

            string? jsonData = JsonConvert.SerializeObject(t);

            return Ok(new ApiResponse(data: jsonData));

        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            throw;

        }

    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult GetTransactions([FromBody] Wallet model)
    {
        try
        {
            if (model.UserId <= 0)
                return BadRequest(new ApiResponse(500));

            var w = _fund.GetWallet((int)model.UserId);

            var t = _fund.GetTransaction(w);

            if (t != null)
            {
                string? jsonData = JsonConvert.SerializeObject(t);
                return Ok(new ApiResponse(data: jsonData));
            }

            return BadRequest(new ApiResponse(500));

        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            throw;

        }
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult GetFinancialReport([FromBody] Wallet model)
    {
        try
        {
            if (model.UserId <= 0)
                return BadRequest(new ApiResponse(500));

            var res = _fund.GetFinancialReport((int)model.UserId);

            if (res != null)
            {
                string? jsonData = JsonConvert.SerializeObject(res);
                return Ok(new ApiResponse(data: jsonData));
            }

            return BadRequest(new ApiResponse(500));

        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            throw;

        }
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult GetBankAccounts(Wallet model)
    {
        try
        {
            var w = _fund.GetWallet((int)model.UserId);

            var t = _fund.GetBankAccounts((int)w.Id);

            if (t == null)
                return BadRequest(new ApiResponse(400));

            string? jsonData = JsonConvert.SerializeObject(t);

            return Ok(new ApiResponse(data: jsonData));

        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            throw;

        }

    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Windrow([FromBody] WalletCurrency model)
    {
        try
        {
            var t = _fund.FindWallerCurrency(model.WalletId, model.CurrencyId);
            if (t == null)
                return BadRequest(new ApiResponse { StatusCode = 400, Message = "کیف پول مبدا پیدا نشد." });

            var wc = _fund.Windrow(model);

            if (wc == null)
                return BadRequest(new ApiResponse(400));

            string? jsonData = JsonConvert.SerializeObject(wc);
            return Ok(new ApiResponse(data: jsonData));

        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            throw;
        }
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Deposit([FromBody] TransactionVM model)
    {
        try
        {
            var t = _fund.FindWallerCurrency(model.WalletId, model.WalletCurrencyId);
            if (t == null)
                return BadRequest(new ApiResponse { StatusCode = 400, Message = "کیف پول مبدا پیدا نشد." });

            var wc = _fund.Deposit(model);
            if (wc == null)
                return BadRequest(new ApiResponse(400));

            string? jsonData = JsonConvert.SerializeObject(wc);
            return Ok(new ApiResponse(data: jsonData));

        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            throw;
        }
    }


    [HttpPost]
    [Route("[action]")]
    public IActionResult AddTransaction([FromBody] TransactionVM model)
    {
        try
        {
            var t = _fund.AddTransaction(model);

            string? jsonData = JsonConvert.SerializeObject(t);
            return Ok(new ApiResponse(data: jsonData));

        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            throw;
        }
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult AddBankAccount([FromBody] WalletBankAccount model)
    {
        try
        {
            var t = _wallet.WalletBankAccounts.FirstOrDefault(x => x.BankAccountNumber == model.BankAccountNumber && x.BankId == model.BankId);

            if (t != null)
                return BadRequest(new ApiResponse { StatusCode = 400, Message = "این حساب قبلا ثبت شده است." });

            var b = _fund.AddBankAccount(model);

            if (b == null)
                return BadRequest(new ApiResponse { StatusCode = 400, Message = "بروز خطا" });

            string? jsonData = JsonConvert.SerializeObject(b);
            return Ok(new ApiResponse(data: jsonData));

        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            throw;
        }
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult ToggleBankCard([FromBody] WalletBankAccount model)
    {
        try
        {
            var t = _wallet.WalletBankAccounts.Find(model.Id);

            if (t == null)
                return BadRequest(new ApiResponse { StatusCode = 400, Message = "حساب بانکی مورد نظر یافت نشد." });

            var b = _fund.ToggleBankCard(model);
            string? jsonData = JsonConvert.SerializeObject(b);
            return Ok(new ApiResponse(data: jsonData));

        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            throw;
        }
    }


    [HttpPost]
    [Route("[action]")]
    public IActionResult ExChange([FromBody] Xchenger model)
    {

        try
        {
            var t = _fund.FindWallerCurrency((long)model.WalletId, model.SourceWalletCurrency);
            if (t == null)
                return BadRequest(new ApiResponse { Message = "کیف پول مبدا پیدا نشد." });

            if (t.Amount < model.SourceAmount)
                return BadRequest(new ApiResponse { Message = "     مقدار درخواستی بیش از موجودی کیف پول مبدا میباشد." });

            var exchange = _fund.AddExchange(model);

            if (exchange != null)
                return Ok(new ApiResponse { StatusCode = 200, Data = JsonConvert.SerializeObject("true") });

            return BadRequest(new ApiResponse(500));
        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            throw;

        }
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult GetExchanges([FromBody] Wallet model)
    {
        try
        {
            if (model.UserId <= 0)
                return BadRequest(new ApiResponse(500));

            var w = _fund.GetWallet((int)model.UserId);

            var t = _fund.GetExchanges((int)w.Id);

            if (t != null)
            {
                string? jsonData = JsonConvert.SerializeObject(t);
                return Ok(new ApiResponse(data: jsonData));
            }

            return BadRequest(new ApiResponse(500));

        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            throw;

        }
    }
}

