using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class TestController : ControllerBase {
    private readonly ITokenService _service;
    public TestController(ITokenService _service) {
        this._service = _service;
    }
    public async Task<IActionResult> Test() {
        var RefreshToken = _service.GenerateRefreshToken();
        Console.WriteLine("The refresh token is ");
        Console.WriteLine(RefreshToken);
        return Ok("heloo sayed");
    }
}