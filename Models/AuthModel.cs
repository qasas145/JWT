public class AuthModel {
    public string Message{get;set;}
    public string Username{get;set;}
    public string Email{get;set;}
    public IList<String> Roles{get;set;}
    public string Token{get;set;}
    public bool IsAuthenticated{get;set;}

}