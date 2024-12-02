using Models;

public interface ITokenService {
    Task<string> GenerateToken(ApplicationUser user);
    string GenerateRefreshToken();
}