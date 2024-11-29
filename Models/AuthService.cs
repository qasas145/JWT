
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JWTOptions _options;
    private readonly ITokenService _tokenService;
    public AuthService(UserManager<ApplicationUser> _userManager, IOptions<JWTOptions> options, ITokenService tokenService) {
        this._userManager = _userManager;
        this._options = options.Value;
        _tokenService = tokenService;
    }

    public async Task<string> Login(LoginModel model)
    {
       var user =  await _userManager.FindByNameAsync(model.UserName);
       if (user is not null) { 
            if (await _userManager.CheckPasswordAsync(user, model.Password)) {
               return await _tokenService.GenerateToken(user);
            }
       }
       return null;

    }

    public async Task<AuthModel> Register(RegisterModel model)
    {
        if (await _userManager.FindByEmailAsync(model.Email) is not null) 
            return new AuthModel(){Message = "The email is already exists"};
        else if (await _userManager.FindByNameAsync(model.UserName) is not null) 
            return new AuthModel() {Message = "The user name is already exists"};
        
        var newUser = new ApplicationUser(){
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.UserName
        };
        var result = await _userManager.CreateAsync(newUser, model.Password);
        if (!result.Succeeded) {
            var errors = string.Empty;
            foreach (var error in result.Errors)
            {
                errors += $"{error.Description},";
            }
            return new AuthModel(){Message = errors};
        }
        await _userManager.AddToRoleAsync(newUser, "User");
        var roles = await _userManager.GetRolesAsync(newUser);
        // generating token;
        var accessToken = await _tokenService.GenerateToken(newUser);
        return new AuthModel(){
            Message = "The user has been created",
            Email = model.Email,
            Username = model.UserName,
            Roles = await _userManager.GetRolesAsync(newUser),
            Token = accessToken,
            IsAuthenticated = true
        };
    }
}
