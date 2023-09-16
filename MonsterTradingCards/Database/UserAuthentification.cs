using MonsterTradingCards.BasicClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Database
{
    public class UserAuthentification
    {
        private List<User> users = new List<User>();

        public void RegisterUser(string username, string password)
        {
            //Simple Password-Hashing
            var salt = GenerateSalt();
            var passwordHash = ComputeHash(password, salt);

            //Save user in db
            users.Add(new User
            {
                UserID = users.Count + 1,
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
                var salt = GetSaltFromHash(user.PasswordHash);
                var hashedPassword = ComputeHash(password, salt);

                if (hashedPassword == user.PasswordHash)
                {
                    //Generate a token + save it for the logged-in user
                    user.Token = GenerateToken();
                    return true;
                }
            }

            return false;
        }

        public bool VerifyToken(string username, string token)
        {
            var user = users.Find(u => u.Username == username);

            if (user != null && user.Token == token)
            {
                return true;
            }

            return false;
        }

        private string GenerateSalt()
        {
            var saltBytes = new byte[16];
            //Secure random number by .NET
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
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
            // Extrahiere das Salz aus einem gehashten Passwort (nicht empfohlen)
            return hashedPassword.Substring(0, 16); // Annahme: Die ersten 16 Zeichen sind das Salz
        }

        private string GenerateToken()
        {
            //Guid = Globally Unique Identifier
            return Guid.NewGuid().ToString();
        }
    }
}
