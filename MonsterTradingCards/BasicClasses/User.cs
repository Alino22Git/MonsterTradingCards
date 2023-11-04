namespace MonsterTradingCards.BasicClasses;

public class User
{
    public int UserId { set; get;}
    public string? Username { set; get;}
    public string? Name { set; get; }
    public string? Bio { set; get; }
    public string? Image { set; get; }
    public string? Password { set; get;}
    public int Money { set; get; }
    public int Elo { set; get; }
    public int Battles { set; get; }

    public User()
    {

    }

    public User(int userId, string? username, string? name, string? bio, string? image, string? password, int money, int elo, int battles)
    {
        UserId = userId;
        Username = username;
        Name = name;
        Bio = bio;
        Image = image;
        Password = password;
        Money = money;
        Elo = elo;
        Battles = battles;
    }

}