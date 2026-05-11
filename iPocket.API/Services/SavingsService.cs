using Microsoft.EntityFrameworkCore;
using iPocket.API.Data;
using iPocket.API.DTOs;
using iPocket.API.Models;
using iPocket.API.Services.Interfaces;

namespace iPocket.API.Services;

public sealed class SavingsService(AppDbContext db) : ISavingsService
{
    // ── List jars (Form912) ──────────────────────────────────────────────────

    public async Task<List<SavingsJarDto>> GetJarsAsync(int userId)
    {
        var wallet = await GetWalletOrThrowAsync(userId);
        var jars   = await db.SavingsJars
            .Include(j => j.Contributions)
            .Where(j => j.WalletId == wallet.Id)
            .ToListAsync();

        return [.. jars.Select(ToDto)];
    }

    // ── Single jar + growth (Form915 / Form916) ───────────────────────────────

    public async Task<SavingsJarDto> GetJarAsync(int userId, int jarId)
    {
        var wallet = await GetWalletOrThrowAsync(userId);
        var jar    = await db.SavingsJars
            .Include(j => j.Contributions)
            .FirstOrDefaultAsync(j => j.Id == jarId && j.WalletId == wallet.Id)
            ?? throw new KeyNotFoundException("Savings jar not found.");

        return ToDto(jar);
    }

    // ── Create jar ────────────────────────────────────────────────────────────

    public async Task<SavingsJarDto> CreateJarAsync(int userId, CreateJarRequest req)
    {
        var wallet = await GetWalletOrThrowAsync(userId);

        var safeRate = req.BaseInterestRate > 1m
            ? req.BaseInterestRate / 100m
            : req.BaseInterestRate;
        safeRate = Math.Clamp(safeRate, 0.001m, 0.50m);

        var jar = new SavingsJar
        {
            WalletId         = wallet.Id,
            Name             = req.Name,
            Description      = req.Description,
            TargetAmount     = req.TargetAmount,
            BaseInterestRate = safeRate,
            TargetYears      = req.TargetYears,
            TargetDate       = DateTime.UtcNow.AddYears(req.TargetYears)
        };

        db.SavingsJars.Add(jar);
        await db.SaveChangesAsync();

        return ToDto(jar);
    }
    

    public async Task<SavingsJarDto> ContributeAsync(int userId, int jarId, ContributeRequest req)
    {
        var wallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId)
            ?? throw new KeyNotFoundException("Wallet not found.");

        if (wallet.Balance < req.Amount)
            throw new InvalidOperationException("Insufficient balance.");

        var jar = await db.SavingsJars
            .Include(j => j.Contributions)
            .FirstOrDefaultAsync(j => j.Id == jarId && j.WalletId == wallet.Id)
            ?? throw new KeyNotFoundException("Savings jar not found.");

        wallet.Balance         -= req.Amount;
        jar.CurrentAmount      += req.Amount;

        db.SavingsContributions.Add(new SavingsContribution
        {
            SavingsJarId = jar.Id,
            Amount       = req.Amount
        });

        db.Transactions.Add(new Transaction
        {
            WalletId    = wallet.Id,
            Type        = TransactionType.SavingsContribution,
            Amount      = req.Amount,
            Description = $"Contributed to {jar.Name}",
            Status      = TransactionStatus.Completed
        });

        await db.SaveChangesAsync();
        return ToDto(jar);
    }

    private static List<GrowthYearDto> CalcGrowth(SavingsJar jar)
    {
        var result = new List<GrowthYearDto>(jar.TargetYears);

        var running = jar.CurrentAmount > 0 ? jar.CurrentAmount : jar.TargetAmount;

        for (int yr = 1; yr <= jar.TargetYears; yr++)
        {
            var rate     = jar.BaseInterestRate + (decimal)(yr - 1) * 0.01m;
            var interest = Math.Round(running * rate, 2);
            running      = Math.Round(running + interest, 2);
            var progress = jar.TargetAmount > 0
                ? Math.Min(100d, (double)(running / jar.TargetAmount * 100))
                : 0d;

            result.Add(new GrowthYearDto(
                yr,
                Math.Round(rate * 100, 1),
                running,
                interest,
                Math.Round(progress, 1)));
        }

        return result;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<Wallet> GetWalletOrThrowAsync(int userId) =>
        await db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId)
        ?? throw new KeyNotFoundException("Wallet not found.");

    private static SavingsJarDto ToDto(SavingsJar j)
    {
        var progress = j.TargetAmount > 0
            ? Math.Min(100d, (double)(j.CurrentAmount / j.TargetAmount * 100))
            : 0d;

        return new SavingsJarDto(
            j.Id, j.Name, j.Description,
            j.TargetAmount, j.CurrentAmount,
            Math.Round(progress, 1),
            j.BaseInterestRate,
            j.TargetYears,
            j.CreatedAt,
            j.TargetDate,
            CalcGrowth(j));
    }
}
