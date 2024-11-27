using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

[ApiController]
public class SigningController :ControllerBase{
    private readonly IAuthService _service;
    public SigningController(IAuthService service){
        _service = service;
    }
    [HttpPost]
    
    [Route("/api/[controller]/register")]
    public async Task<IActionResult> Register(RegisterModel model){
        var result = await _service.Register(model);
        return Ok(result);
    }
    
    [HttpPost]
    [Route("/api/[controller]/login")]
    public async Task<IActionResult> Login(LoginModel model){
        var result = await _service.Login(model);
        if (result == null) {
            return Unauthorized();
        }
        return Ok(result);
    }
    [HttpGet]
    [Authorize(Roles ="User")]
    [Route("/api/[controller]/test")]
    public async Task<IActionResult> Test() {
        var email = Request.HttpContext.User.Claims.FirstOrDefault(c=>c.Type == ClaimTypes.Name);
        if (email is not null)
            Console.WriteLine(email.Value);
        return Ok("Hello User, you are authorized");
    }

}