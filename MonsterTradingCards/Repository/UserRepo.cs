using System.Data;
using MonsterTradingCards.BasicClasses;
using Npgsql;

namespace MonsterTradingCards.Repository;

public class UserRepo : IRepository<User>
{
    private readonly string ConnectionString;

    public UserRepo(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public IEnumerable<User> GetAll()
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
                        int money = reader.GetInt32(6);

                        data.Add(new User(userId, username, name, bio, image, password,money));
                    }
                }
            }
        }

        return data;
    }


    public void Update(User u)
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

    public void Delete(User u)
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
        Console.WriteLine(db);
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
}