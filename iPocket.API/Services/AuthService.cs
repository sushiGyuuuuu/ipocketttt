using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using iPocket.API.Data;
using iPocket.API.DTOs;
using iPocket.API.Models;
using iPocket.API.Services.Interfaces;

namespace iPocket.API.Services;

public sealed class AuthService(
    AppDbContext db,
    IConfiguration config,
    ILogger<AuthService> log) : IAuthService
{
    // ── OTP ──────────────────────────────────────────────────────────────────

    public async Task<(bool Ok, string Message)> SendOtpAsync(string mobileNumber, string purpose)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.MobileNumber == mobileNumber);

        if (user is null)
        {
            if (purpose == "login")
            {
                log.LogInformation("OTP requested for unknown number {Num} (login)", mobileNumber);
                return (true, "If that number is registered, a code has been sent.");
            }
            user = new User
            {
                FullName     = "Pending",
                MobileNumber = mobileNumber,
                PasswordHash = string.Empty,
                PinHash      = string.Empty,
                IsVerified   = false
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        var old = await db.OtpCodes
            .Where(o => o.UserId == user.Id && o.Purpose == purpose && !o.IsUsed)
            .ToListAsync();
        old.ForEach(o => o.IsUsed = true);

        var raw = Random.Shared.Next(1000, 9999).ToString("D4");

        db.OtpCodes.Add(new OtpCode
        {
            UserId    = user.Id,
            Code      = BCrypt.Net.BCrypt.HashPassword(raw),
            Purpose   = purpose,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        });

        await db.SaveChangesAsync();

        log.LogWarning("[DEV-OTP] {Num} → {Code}", mobileNumber, raw);

        return (true, $"OTP sent. (dev mode — code: {raw})");
    }

    public async Task<bool> VerifyOtpAsync(string mobileNumber, string code)
    {
        var otp = await db.OtpCodes
            .Include(o => o.User)
            .Where(o => o.User!.MobileNumber == mobileNumber
                        && !o.IsUsed
                        && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp is null || !BCrypt.Net.BCrypt.Verify(code, otp.Code))
            return false;

        otp.IsUsed = true;
        if (otp.User is not null)
            otp.User.IsVerified = true;

        await db.SaveChangesAsync();
        return true;
    }

    // ── Register (Form7 + Form8) ──────────────────────────────────────────────

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        var existing = await db.Users.FirstOrDefaultAsync(u => u.MobileNumber == req.MobileNumber);

        User user;
        if (existing is not null && !string.IsNullOrEmpty(existing.PasswordHash))
            throw new InvalidOperationException("Mobile number is already registered.");

        if (existing is not null)
        {
            existing.FullName     = req.FullName;
            existing.Email        = req.Email;
            existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
            existing.PinHash      = BCrypt.Net.BCrypt.HashPassword(req.Pin);
            user = existing;
        }
        else
        {
            user = new User
            {
                FullName     = req.FullName,
                MobileNumber = req.MobileNumber,
                Email        = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                PinHash      = BCrypt.Net.BCrypt.HashPassword(req.Pin),
            };
            db.Users.Add(user);
        }

        await db.SaveChangesAsync();

        if (!await db.Wallets.AnyAsync(w => w.UserId == user.Id))
        {
            var suffix = Random.Shared.Next(1000, 9999);
            db.Wallets.Add(new Wallet
            {
                UserId     = user.Id,
                CardNumber = $"**** **** **** {suffix}"
            });
            await db.SaveChangesAsync();
        }

        return new AuthResponse(GenerateToken(user), ToDto(user));
    }


    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.MobileNumber == req.MobileNumber)
            ?? throw new UnauthorizedAccessException("Invalid mobile number or password.");

        if (string.IsNullOrEmpty(user.PasswordHash) ||
            !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid mobile number or password.");

        return new AuthResponse(GenerateToken(user), ToDto(user));
    }

    // ── JWT ───────────────────────────────────────────────────────────────────

    public string GenerateToken(User user)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.MobilePhone,    user.MobileNumber),
            new(ClaimTypes.Name,           user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var token = new JwtSecurityToken(
            issuer:             config["Jwt:Issuer"],
            audience:           config["Jwt:Audience"],
            claims:             claims,
            expires:            DateTime.UtcNow.AddDays(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static UserDto ToDto(User u) => new(
        u.Id, u.FullName, u.MobileNumber, u.Email,
        u.IsVerified, u.KycStatus.ToString(), u.CreatedAt);
}
