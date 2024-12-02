using Models;

public interface IAuthService {
    Task<AuthModel> Register(RegisterModel model);
    Task<AuthModel> Login(LoginModel model);
    Task<AuthModel> Refresh(ApplicationUser user, string token);
}