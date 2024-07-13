namespace Wallet.Models;
public class DepositRequest
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
}

public class WithdrawRequest
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
}

 