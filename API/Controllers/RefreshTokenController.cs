using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

[ApiController]
[Route("/api/[controller]")]
public class RefreshTokenController : ControllerBase {
    private readonly IAuthService _service;
    private readonly UserManager<ApplicationUser> _userManager;
    public RefreshTokenController(IAuthService _service, UserManager<ApplicationUser> _userManager) {
        this._service = _service;
        this._userManager = _userManager;
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken() {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken is null) 
            return BadRequest("No refresh token");

        var user = _userManager.Users.SingleOrDefault(u=>u.RefreshTokens.Any(r=>r.Token == refreshToken));

        if (user is null) 
            return BadRequest("No user with this token ");

        var result = await _service.Refresh(user,refreshToken);
        if (result is  null) 
            return BadRequest(result.Message);
        AddRefreshTokenToCookies(result.RefreshToken, result.RefreshTokenExpiresOn);
        return Ok(result);
    }
     public void AddRefreshTokenToCookies(string refreshToken, DateTime expiresOn) {
        var cookieOptions = new CookieOptions() {
            Expires = expiresOn,
            HttpOnly = true
        };
        Response.Cookies.Delete("refreshToken");
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}