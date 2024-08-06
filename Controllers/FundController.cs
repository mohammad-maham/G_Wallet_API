using G_Wallet_API.BusinessLogic.Interfaces;
using G_Wallet_API.Common;
using G_Wallet_API.Models;
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
    public async Task<IActionResult> GetWallet([FromBody] Wallet model)
    {
        try
        {
            if(model.UserId <= 0)
                return BadRequest(new ApiResponse(500));

            var t = await _fund.GetWallet((int)model.UserId);

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
    public async Task<IActionResult> GetWalletCurrency([FromBody] Wallet model)
    {
        try
        {

            var t = await _fund.GetWalletCurrency((int)model.UserId);

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
    public async Task<IActionResult> Windrow([FromBody] WalletCurrency model)
    {
        try
        {
            var t = await _wallet.WalletCurrencies.FirstOrDefaultAsync(x => x.WalletId == model.WalletId);

            if (t == null || t.Amount < model.Amount)
                return BadRequest(new ApiResponse(700));

            //await _fund.Windrow(model);

            string? jsonData = JsonConvert.SerializeObject(model);
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
    public async Task<IActionResult> Deposit([FromQuery] WalletCurrency model)
    {
        try
        {
            var t = await _fund.FindWallerCurrencyAsync(model.WalletId, model.CurrencyId);
            if (t == null)
                return BadRequest(new ApiResponse { Message = "کیف پول مبدا پیدا نشد." });

            //await _fund.Deposit(model);

            string? jsonData = JsonConvert.SerializeObject(model);
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
    public async Task<IActionResult> AddTransaction([FromQuery] Transaction model)
    {
        try
        {
            await _fund.AddTransaction(model);

            string? jsonData = JsonConvert.SerializeObject(model);
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
    public async Task<IActionResult> AddBankAccount([FromQuery] WalletBankAccount model)
    {
        try
        {
            await _fund.AddBankAccount(model);

            string? jsonData = JsonConvert.SerializeObject(model);
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
    public async Task<IActionResult> ExChange([FromQuery] Xchenger model)
    {

        try
        {
            var t = await _fund.FindWallerCurrencyAsync(model.WalletId,model.SourceWalletCurrency);
            if(t==null)
                return BadRequest(new ApiResponse { Message = "کیف پول مبدا پیدا نشد." });

            if (t.Amount<model.SourceAmount)
                return BadRequest(new ApiResponse { Message = "     مقدار درخواستی بیش از موجودی کیف پول مبدا میباشد." });

            var exchange = await _fund.AddExchange(model);

            if (exchange != null)
                return Ok(new ApiResponse());

            return BadRequest(new ApiResponse(500));
        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            throw;

        }
    }
}

