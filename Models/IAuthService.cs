public interface IAuthService {
    Task<AuthModel> Register(RegisterModel model);
    Task<string> Login(LoginModel model);
}