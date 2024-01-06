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
    public int Wins { set; get; }
    public int RoundsPlayed { set; get; }
    public int RoundsWon { set; get; }
    public int RoundsLost { set; get; }


    public User()
    {

    }

    public User(int userId, string? username, string? name, string? bio, string? image, string? password, int money, int elo, int battles,int wins, int roundsPlayed, int roundsWon, int roundsLost)
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
        Wins = wins;
        RoundsPlayed = roundsPlayed;
        RoundsWon = roundsWon;
        RoundsLost = roundsLost;
    }

}