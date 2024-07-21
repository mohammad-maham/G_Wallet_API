using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using G_Wallet_API.Models;
using G_Wallet_API.BusinessLogic.Interfaces;

namespace G_Wallet_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FundController : ControllerBase
{
    private readonly IFund _fund;

    private readonly ILogger<FundController> _logger;

    public FundController(IFund  fund , ILogger<FundController> logger)
    {
        _fund = fund;
        _logger = logger;
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SignIn([FromQuery] WalletBankAccount wallet)
    {
        
        return Ok();
    }


}