using Wallet.BusinessLogic.Interfaces;
using Wallet.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Wallet.Controllers;


[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    private readonly ILogger<WalletController> _logger;

    public WalletController(IWalletService walletService, ILogger<WalletController> logger)
    {
        _walletService = walletService;
        _logger = logger;
    }

    [HttpPost("deposit")]
    public IActionResult Deposit([FromBody] DepositRequest request)
    {
        _walletService.Deposit(request.UserId, request.Amount);
        return Ok();
    }

    [HttpPost("withdraw")]
    public IActionResult Withdraw([FromBody] WithdrawRequest request)
    {
        _walletService.Withdraw(request.UserId, request.Amount);
        return Ok();
    }

    [HttpGet("balance/{userId}")]
    public IActionResult GetBalance(int userId)
    {
        var balance = _walletService.GetBalance(userId);
        return Ok(new { Balance = balance });
    }


}