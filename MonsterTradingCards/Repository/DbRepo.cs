using System.Collections.Generic;
using System.Data;
using MonsterTradingCards.BasicClasses;
using Npgsql;

namespace MonsterTradingCards.Repository;

public class DbRepo
{
    private readonly string ConnectionString;

    public DbRepo(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public IEnumerable<User> GetAllUsers()
    {
        var data = new List<User>();
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"SELECT *
                                        FROM Player";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var userId = reader.GetInt32(0);
                        var username = reader.GetString(1);
                        string name = null;
                        string bio = null;
                        string image = null;
                        var password = reader.GetString(5);
                        if (!reader.IsDBNull(2)) name = reader.GetString(2);
                        if (!reader.IsDBNull(3)) bio = reader.GetString(3);
                        if (!reader.IsDBNull(4)) image = reader.GetString(4);
                        var money = reader.GetInt32(6);

                        data.Add(new User(userId, username, name, bio, image, password, money));
                    }
                }
            }
        }

        return data;
    }


    public void UpdateUser(User u)
    {
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"UPDATE Player
                                            SET username = @uname, name=@name, bio=@bio, image=@image, password = @pass, money = @money
                                            WHERE userId = @id";


                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "id";
                idParameter.Value = u.UserId;
                command.Parameters.Add(idParameter);

                var unameParameter = command.CreateParameter();
                unameParameter.ParameterName = "uname";
                unameParameter.Value = u.Username;
                command.Parameters.Add(unameParameter);

                var nameParameter = command.CreateParameter();
                nameParameter.ParameterName = "name";
                if (u.Name != null)
                    nameParameter.Value = u.Name;
                else
                    nameParameter.Value = DBNull.Value;
                command.Parameters.Add(nameParameter);

                var bioParameter = command.CreateParameter();
                bioParameter.ParameterName = "bio";
                if (u.Bio != null)
                    bioParameter.Value = u.Bio;
                else
                    bioParameter.Value = DBNull.Value;
                command.Parameters.Add(bioParameter);

                var imageParameter = command.CreateParameter();
                imageParameter.ParameterName = "image";
                if (u.Image != null)
                    imageParameter.Value = u.Image;
                else
                    //DBNull is to represent null in the database
                    imageParameter.Value = DBNull.Value;
                command.Parameters.Add(imageParameter);

                var passParameter = command.CreateParameter();
                passParameter.ParameterName = "pass";
                passParameter.Value = u.Password;
                command.Parameters.Add(passParameter);

                var moneyParameter = command.CreateParameter();
                moneyParameter.ParameterName = "money";
                moneyParameter.Value = u.Money;
                command.Parameters.Add(moneyParameter);

                command.ExecuteNonQuery();
            }
        }
    }

    public void DeleteUser(User u)
    {
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"DELETE FROM Player WHERE userId = @id";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "id";
                parameter.Value = u.UserId;
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }
        }
    }

    public static void InitDb(string connectionString)
    {
        //Creates a connection string
        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        //To connect to db
        var db = builder.Database;
        builder.Remove("Database");
        var connstr = builder.ToString();

        using (IDbConnection connection = new NpgsqlConnection(connstr))
        {
            connection.Open();

            //IDbCommand represents a SQL statement
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"DROP DATABASE IF EXISTS \"{db}\" WITH (force)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"CREATE DATABASE \"{db}\"";
                cmd.ExecuteNonQuery();
            }

            //Changes location to db
            connection.ChangeDatabase(db);

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Player (
                            userId SERIAL PRIMARY KEY, 
                            username VARCHAR(50) NOT NULL,
                            name VARCHAR(50),
                            bio VARCHAR(50),
                            image VARCHAR(50),
                            password VARCHAR(255) NOT NULL,
                            money INT
                        )
                    ";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Card (
                            cardId VARCHAR(50) PRIMARY KEY, 
                            name VARCHAR(50) NOT NULL,
                            damage INT,
                            deck INT
                        )
                    ";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS UserCard (
                            Id SERIAL PRIMARY KEY,
                            userId INT REFERENCES Player(userId),
                            cardId VARCHAR(50) REFERENCES Card(cardId)
                        )
                    ";
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void AddUserCredentials(User u)
    {
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"INSERT INTO Player (username, password, money)
                                    VALUES (@uname, @pass, @money)";


                //Parameter for username
                var unameParameter = command.CreateParameter();
                unameParameter.ParameterName = "uname";
                unameParameter.Value = u.Username;
                command.Parameters.Add(unameParameter);

                //Parameter for password
                var passParameter = command.CreateParameter();
                passParameter.ParameterName = "pass";
                passParameter.Value = u.Password;
                command.Parameters.Add(passParameter);

                var moneyParameter = command.CreateParameter();
                moneyParameter.ParameterName = "money";
                moneyParameter.Value = 20;
                command.Parameters.Add(moneyParameter);

                command.ExecuteNonQuery();
            }
        }
    }

    public IEnumerable<Card>? GetCardPackage()
    {
        var data = new List<Card>();
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"SELECT Card.*
                                        FROM Card
                                        LEFT JOIN UserCard ON Card.cardId = UserCard.cardId
                                        WHERE UserCard.cardId IS NULL;";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cardId = reader.GetString(0);
                        var name = reader.GetString(1);
                        var damage = reader.GetInt32(2);
                        var deck = reader.GetInt32(3);

                        data.Add(new Card(cardId, name, damage, deck));
                    }
                }
            }
        }

        if (data.Count() < 5)
        {
            return null;
        }
        return data.GetRange(0,5);
    }
    public void AddCard(Card c)
    {
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"INSERT INTO Card (cardid, name, damage,deck)
                                    VALUES (@id, @name, @damage, 0)";


                //Parameter for username
                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "id";
                idParameter.Value = c.Id;
                command.Parameters.Add(idParameter);

                //Parameter for password
                var nameParamer = command.CreateParameter();
                nameParamer.ParameterName = "name";
                nameParamer.Value = c.Name;
                command.Parameters.Add(nameParamer);

                var damageParameter = command.CreateParameter();
                damageParameter.ParameterName = "damage";
                damageParameter.Value = c.Damage;
                command.Parameters.Add(damageParameter);

                command.ExecuteNonQuery();
            }
        }
    }

    public IEnumerable<Card> GetAllCards()
    {
        var data = new List<Card>();
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"SELECT *
                                        FROM Card";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cardId = reader.GetString(0);
                        var name = reader.GetString(1);
                        var damage = reader.GetInt32(2);
                        var deck = reader.GetInt32(3);

                        data.Add(new Card(cardId, name, damage,deck));
                    }
                }
            }
        }
        return data;
    }


    public void UpdateCard(Card c)
    {
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"UPDATE Card
                                            SET name = @name, damage = @damage, deck = @deck
                                            WHERE cardId = @id";


                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "id";
                idParameter.Value = c.Id;
                command.Parameters.Add(idParameter);

                var nameParameter = command.CreateParameter();
                nameParameter.ParameterName = "name";
                nameParameter.Value = c.Name;
                command.Parameters.Add(nameParameter);

                var damageParameter = command.CreateParameter();
                damageParameter.ParameterName = "damage";
                damageParameter.Value = c.Damage;
                command.Parameters.Add(damageParameter);

                var deckParameter = command.CreateParameter();
                deckParameter.ParameterName = "deck";
                deckParameter.Value = c.Deck;
                command.Parameters.Add(deckParameter);

                command.ExecuteNonQuery();
            }
        }
    }

    public void DeleteCard(Card c)
    {
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"DELETE FROM Card WHERE cardId = @id";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "id";
                parameter.Value = c.Id;
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }
        }
    }

    public void UserAquireCards(User user)
    {
        List<Card> cards = (List<Card>) GetCardPackage();
        foreach(var card in cards) {
            using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"INSERT INTO UserCard(userId, cardId) 
                                        VALUES(@uid, @cid)";

                    var uidParameter = command.CreateParameter();
                    uidParameter.ParameterName = "uid";
                    uidParameter.Value = user.UserId;
                    command.Parameters.Add(uidParameter);

                    var cidParameter = command.CreateParameter();
                    cidParameter.ParameterName = "cid";
                    cidParameter.Value = card.Id;
                    command.Parameters.Add(cidParameter);

                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public IEnumerable<Card> UserGetCards(User user)
    {
        var data = new List<Card>();
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"SELECT Card.*
                                        FROM Card
                                        JOIN UserCard ON Card.cardId = UserCard.cardId
                                        WHERE UserCard.userId = @id";
                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "id";
                idParameter.Value = user.UserId;
                command.Parameters.Add(idParameter);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cardId = reader.GetString(0);
                        var name = reader.GetString(1);
                        var damage = reader.GetInt32(2);
                        var deck = reader.GetInt32(3);

                        data.Add(new Card(cardId, name, damage, deck));
                    }
                }
            }
        }
        if (data.Count() == 0)
        {
            return null;
        }
        return data;
    }
    public IEnumerable<Card> UserGetDeck(User user)
    {
        var data = new List<Card>();
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"SELECT Card.*
                                        FROM Card
                                        JOIN UserCard ON Card.cardId = UserCard.cardId
                                        WHERE UserCard.userId = @id AND Card.deck = 1";
                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "id";
                idParameter.Value = user.UserId;
                command.Parameters.Add(idParameter);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cardId = reader.GetString(0);
                        var name = reader.GetString(1);
                        var damage = reader.GetInt32(2);
                        var deck = reader.GetInt32(3);

                        data.Add(new Card(cardId, name, damage, deck));
                    }
                }
            }
        }
        if (data.Count() == 0)
        {
            return null;
        }
        return data;
    }
}