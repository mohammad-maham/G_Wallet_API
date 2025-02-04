using G_Wallet_API.Models;

namespace DefaultNamespace;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class BankController : ControllerBase
{
    private readonly GWalletDbContext _context;

    public BankController(GWalletDbContext context)
    {
        _context = context;
    }

    // Get all banks
    [HttpGet]
    public async Task<IActionResult> GetBanks()
    {
        var banks = await _context.Banks.ToListAsync();
        return Ok(banks);
    }

    // Get a bank by Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBankById(int id)
    {
        var bank = await _context.Banks.FindAsync(id);
        if (bank == null)
        {
            return NotFound(new { Message = "Bank not found" });
        }
        return Ok(bank);
    }

    // Create a new bank
    [HttpPost]
    public async Task<IActionResult> CreateBank([FromBody] Bank bank)
    {
        if (bank == null)
        {
            return BadRequest(new { Message = "Invalid bank data" });
        }

        await _context.Banks.AddAsync(bank);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBankById), new { id = bank.Id }, bank);
    }

    // Update an existing bank
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBank(int id, [FromBody] Bank updatedBank)
    {
        if (updatedBank == null || id != updatedBank.Id)
        {
            return BadRequest(new { Message = "Invalid bank data" });
        }

        var existingBank = await _context.Banks.FindAsync(id);
        if (existingBank == null)
        {
            return NotFound(new { Message = "Bank not found" });
        }

        existingBank.Name = updatedBank.Name;
        existingBank.Status = updatedBank.Status;
        existingBank.LogoPath = updatedBank.LogoPath;

        await _context.SaveChangesAsync();
        return Ok(existingBank);
    }

    // Delete a bank
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBank(int id)
    {
        var bank = await _context.Banks.FindAsync(id);
        if (bank == null)
        {
            return NotFound(new { Message = "Bank not found" });
        }

        _context.Banks.Remove(bank);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Bank deleted successfully" });
    }
}
