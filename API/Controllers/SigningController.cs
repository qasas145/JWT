using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Models;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

[ApiController]
[Route("/api/[controller]")]
public class SigningController :ControllerBase{
    private readonly IAuthService _service;
    public SigningController(IAuthService service){
        _service = service;
    }
    [HttpPost("register")]
    
    public async Task<IActionResult> Register(RegisterModel model){
        if (!ModelState.IsValid)
            return BadRequest(ModelState); // this will return the error 
        var result = await _service.Register(model);
        if (!result.IsAuthenticated)    
            return BadRequest(result.Message);
        AddRefreshTokenToCookies(result.RefreshToken, result.RefreshTokenExpiresOn);
        return Ok(result);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model){
        var result = await _service.Login(model);
        if (result == null) {
            return Unauthorized();
        }
        AddRefreshTokenToCookies(result.RefreshToken, result.RefreshTokenExpiresOn);
        return Ok(result);
    }
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout() {
        var userClaims = Request.HttpContext.User.Claims;
        var userNameClaim = userClaims.FirstOrDefault(c=>c.Type == JwtRegisteredClaimNames.Name);
        var result = await _service.Logout(userNameClaim.Value);

        return Ok(result.Message);
    }

    [HttpPost("test")]
    [Authorize(Roles ="User")]
    public async Task<IActionResult> Test() {
        var claims = Request.HttpContext.User.Claims;
        var name = Request.HttpContext.User.Claims.FirstOrDefault(c=>c.Type == JwtRegisteredClaimNames.Name);
        return Ok("Hello User, you are authorized");
    }
    public void AddRefreshTokenToCookies(string refreshToken, DateTime expiresOn) {
        var cookieOptions = new CookieOptions() {
            Expires = expiresOn,
            HttpOnly = true
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

}