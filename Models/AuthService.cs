
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

    public async Task<AuthModel> Login(LoginModel model)
    {
       var user =  await _userManager.FindByNameAsync(model.UserName);
       if (user is null) 
            return new AuthModel() {
                Message = "The username is invalid"
            };
        
        if (!await _userManager.CheckPasswordAsync(user, model.Password)) 
            return new AuthModel() {Message = "The password is invalid "};
        
        var refreshToken = GenerateRefreshToken();
        user.RefreshTokens.Add(refreshToken);
        await _userManager.UpdateAsync(user);
        
        return new AuthModel() {
            Token = await _tokenService.GenerateToken(user),
            Message = "The user is authenticated",
            IsAuthenticated = true,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresOn = refreshToken.ExpiresOn

        };

    }

    public async Task<AuthModel> Refresh(ApplicationUser user,string token)
    {
        var refreshToken = user.RefreshTokens.FirstOrDefault(r=>r.Token == token && r.IsActive == true);
        var authModel = new AuthModel();
        if (refreshToken == null) 
            authModel.Message = "The refresh token is invalid ";
        else {

            var newRefreshToken = GenerateRefreshToken();
            
            authModel.Token =  await _tokenService.GenerateToken(user);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            authModel.Message = "The user is authenticated";
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiresOn = refreshToken.ExpiresOn.ToLocalTime();
            authModel.IsAuthenticated = true;
            refreshToken.RevokedOn = DateTime.UtcNow;
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);
        };
        return authModel;
        
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
        // generating token;
        var accessToken = await _tokenService.GenerateToken(newUser);
        // part related to refresh token
        var refreshToken = new RefreshToken() {
            Token = _tokenService.GenerateRefreshToken(),
            CreatedOn = DateTime.UtcNow,
            ExpiresOn =DateTime.UtcNow.AddDays(_options.RefreshTokenExpiresInDays)
        };
        newUser.RefreshTokens.Add(refreshToken);
        await _userManager.UpdateAsync(newUser);
        return new AuthModel(){
            Message = "The user has been created",
            Email = model.Email,
            Username = model.UserName,
            Roles = await _userManager.GetRolesAsync(newUser),
            Token = accessToken,
            IsAuthenticated = true,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresOn = refreshToken.ExpiresOn.ToLocalTime()
        };
    }
    public RefreshToken GenerateRefreshToken() {
        return new RefreshToken() {
            Token = _tokenService.GenerateRefreshToken(),
            CreatedOn = DateTime.UtcNow,
            ExpiresOn = DateTime.UtcNow.AddDays(_options.RefreshTokenExpiresInDays)
        };
    }


}
