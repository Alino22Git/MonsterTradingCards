using System.Security.Cryptography;
using System.Text;
using MonsterTradingCards.BasicClasses;

namespace MonsterTradingCards.Database;

public class UserAuthentification
{
    private readonly List<User> users = new();

    public void RegisterUser(string username, string password)
    {
        //Simple Password-Hashing
        var salt = GenerateSalt(16);
        var passwordHash = ComputeHash(password, salt);

        //Save user in db
        users.Add(new User
        {
            UserId = users.Count + 1,
            Username = username,
            PasswordHash = passwordHash,
            //While registration the user does not have a token
            Token = null
        });
    }

    public bool LoginUser(string username, string password)
    {
        var user = users.Find(x => x.Username == username);

        if (user != null)
        {
            //Test Password
            if (user.PasswordHash != null)
            {
                var salt = GetSaltFromHash(user.PasswordHash);
                var hashedPassword = ComputeHash(password, salt);

                if (hashedPassword == user.PasswordHash)
                {
                    //Generate a token + save it for the logged-in user
                    user.Token = GenerateToken();
                    return true;
                }
            }
        }

        return false;
    }

    public bool VerifyToken(string username, string token)
    {
        var user = users.Find(u => u.Username == username);

        if (user != null && user.Token == token) return true;

        return false;
    }

    public static string GenerateSalt(int length)
    {
        var randomBytes = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return BitConverter.ToString(randomBytes).Replace("-", "");
    }

    private string ComputeHash(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var saltedPassword = Encoding.UTF8.GetBytes(password + salt);
            var hashBytes = sha256.ComputeHash(saltedPassword);
            return Convert.ToBase64String(hashBytes);
        }
    }

    private string GetSaltFromHash(string hashedPassword)
    {
        //Extrate salt from Hash (implification that first 16 bytes are salt)
        return hashedPassword.Substring(0, 16);
    }

    private string GenerateToken()
    {
        //Guid = Globally Unique Identifier
        return Guid.NewGuid().ToString();
    }
}