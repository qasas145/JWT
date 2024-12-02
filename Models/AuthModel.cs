using System.Text.Json.Serialization;

public class AuthModel {
    public string Message{get;set;}
    public string Username{get;set;}
    public string Email{get;set;}
    public IList<String> Roles{get;set;}
    public string Token{get;set;}
    public bool IsAuthenticated{get;set;}
    [JsonIgnore]
    
    public string RefreshToken{get;set;}
    public DateTime RefreshTokenExpiresOn{get;set;}

}