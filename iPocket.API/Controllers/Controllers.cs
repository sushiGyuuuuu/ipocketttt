using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iPocket.API.DTOs;
using iPocket.API.Services.Interfaces;

namespace iPocket.API.Controllers;

// ═══════════════════════════════════════════════════════════════════════════════
//  AUTH   Form4 → Form5 → Form6 → Form7 → Form8
// ═══════════════════════════════════════════════════════════════════════════════

[ApiController, Route("api/auth"), Produces("application/json")]
public sealed class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp(SendOtpRequest req)
    {
        var (ok, msg) = await auth.SendOtpAsync(req.MobileNumber, req.Purpose);
        return ok
            ? Ok(new ApiResponse<object>(true, msg, null))
            : BadRequest(new ErrorResponse(msg));
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(VerifyOtpRequest req)
    {
        var ok = await auth.VerifyOtpAsync(req.MobileNumber, req.Code);
        return ok
            ? Ok(new ApiResponse<object>(true, "OTP verified.", null))
            : BadRequest(new ErrorResponse("Invalid or expired OTP."));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        try
        {
            var result = await auth.RegisterAsync(req);
            return StatusCode(201, new ApiResponse<AuthResponse>(true, "Account created.", result));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse(ex.Message));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        try
        {
            var result = await auth.LoginAsync(req);
            return Ok(new ApiResponse<AuthResponse>(true, "Login successful.", result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse(ex.Message));
        }
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
//  USERS   Form917 Profile · Form919 Settings
// ═══════════════════════════════════════════════════════════════════════════════

[ApiController, Route("api/users"), Authorize, Produces("application/json")]
public sealed class UsersController(IUserService users) : ControllerBase
{
    private int Uid => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var dto = await users.GetProfileAsync(Uid);
        return Ok(new ApiResponse<UserDto>(true, null, dto));
    }

    [HttpPatch("me")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest req)
    {
        var dto = await users.UpdateProfileAsync(Uid, req);
        return Ok(new ApiResponse<UserDto>(true, "Profile updated.", dto));
    }

    [HttpPost("me/verify-pin")]
    public async Task<IActionResult> VerifyPin(VerifyPinRequest req)
    {
        var ok = await users.VerifyPinAsync(Uid, req.Pin);
        return ok
            ? Ok(new ApiResponse<object>(true, "PIN verified.", null))
            : Unauthorized(new ErrorResponse("Incorrect PIN."));
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
//  WALLET   Form911 Home · Form913 Deposit · Form914 Send · Form918 History
// ═══════════════════════════════════════════════════════════════════════════════

[ApiController, Route("api/wallet"), Authorize, Produces("application/json")]
public sealed class WalletController(IWalletService wallet) : ControllerBase
{
    private int Uid => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetWallet()
    {
        var dto = await wallet.GetWalletAsync(Uid);
        return Ok(new ApiResponse<WalletDto>(true, null, dto));
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit(DepositRequest req)
    {
        var tx = await wallet.DepositAsync(Uid, req);
        return Ok(new ApiResponse<TransactionDto>(true, "Deposit successful.", tx));
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(SendMoneyRequest req)
    {
        var tx = await wallet.SendMoneyAsync(Uid, req);
        return Ok(new ApiResponse<TransactionDto>(true, "Money sent.", tx));
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] string? filter = "All",
        [FromQuery] int     page   = 1,
        [FromQuery] int     size   = 20)
    {
        var result = await wallet.GetTransactionsAsync(Uid, filter, page, size);
        return Ok(new ApiResponse<TransactionPageDto>(true, null, result));
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
//  SAVINGS   Form912 Jar list · Form915 Jar of Joy · Form916 Growth Details
// ═══════════════════════════════════════════════════════════════════════════════

[ApiController, Route("api/savings"), Authorize, Produces("application/json")]
public sealed class SavingsController(ISavingsService savings) : ControllerBase
{
    private int Uid => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetJars()
    {
        var list = await savings.GetJarsAsync(Uid);
        return Ok(new ApiResponse<List<SavingsJarDto>>(true, null, list));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetJar(int id)
    {
        var jar = await savings.GetJarAsync(Uid, id);
        return Ok(new ApiResponse<SavingsJarDto>(true, null, jar));
    }

    [HttpPost]
    public async Task<IActionResult> CreateJar(CreateJarRequest req)
    {
        var jar = await savings.CreateJarAsync(Uid, req);
        return StatusCode(201, new ApiResponse<SavingsJarDto>(true, "Jar created.", jar));
    }

    [HttpPost("{id:int}/contribute")]
    public async Task<IActionResult> Contribute(int id, ContributeRequest req)
    {
        var jar = await savings.ContributeAsync(Uid, id, req);
        return Ok(new ApiResponse<SavingsJarDto>(true, "Contribution added.", jar));
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
//  KYC   Form9 Verify Identity · Form910 Select Category
// ═══════════════════════════════════════════════════════════════════════════════

[ApiController, Route("api/kyc"), Authorize, Produces("application/json")]
public sealed class KycController(IKycService kyc) : ControllerBase
{
    private int Uid => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetStatus()
    {
        var dto = await kyc.GetStatusAsync(Uid);
        return Ok(new ApiResponse<KycStatusDto>(true, null, dto));
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit(SubmitKycRequest req)
    {
        var doc = await kyc.SubmitDocumentAsync(Uid, req);
        return StatusCode(201, new ApiResponse<KycDocumentDto>(true, "Document submitted.", doc));
    }
}
