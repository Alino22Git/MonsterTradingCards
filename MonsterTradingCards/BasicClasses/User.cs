namespace MonsterTradingCards.BasicClasses;

public class User
{
    public int UserId { set; get;}
    public string? Username { set; get;}
    public string? Name { set; get; }
    public string? Bio { set; get; }
    public string? Image { set; get; }
    public string? Password { set; get;}
    public string Token { get; set; }

    public User()
    {
    }

    public User(int userId, string? username, string? name, string? bio, string? image, string? password, string token)
    {
        UserId = userId;
        Username = username;
        Name = name;
        Bio = bio;
        Image = image;
        Password = password;
        Token = token;
    }
}