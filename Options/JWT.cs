
public class JWTOptions {
    public string Key{get;set;}
    public string Issuer{get;set;}
    public string Audience{get;set;}
    public int ExpireinDays{get;set;}
    public int ExpireinMinutes{get;set;}
    public int RefreshTokenExpiresInDays{get;set;}
}