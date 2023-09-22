using System.Security.Cryptography;

namespace MonsterTradingCards.BasicClasses;

public class User
{
    public int UserId { get;}
    public string Username { get;}
    public string PasswordHash { get;}
    public string? Token { get; set; }

    public User()
    {
    }

    public User(int userId, string username, string passwordHash, string? token)
    {
        UserId = userId;
        Username = username;
        PasswordHash = passwordHash;
        Token = token;
    }

    public override bool Equals(object o)
    {
        if (this == o)
        {
            return true;
        }
        if (o.GetType() != this.GetType())
        {
            return false;
        }

        var that = (User)o;
        return this.UserId.Equals(that.UserId)
               && this.Username.Equals(that.Username)
               && this.PasswordHash.Equals(that.PasswordHash);
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(UserId, Username, PasswordHash);
    }

    public override string ToString()
    {
        return $"User{{UserId='{string.Join(", ", UserId)}', Username='{Username}', PasswordHash='{PasswordHash}', Token={Token}'}}";
    }
}