using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Query;
using MonsterTradingCards.BasicClasses;
using Newtonsoft.Json.Linq;
using Npgsql;
using NpgsqlTypes;

namespace MonsterTradingCards.Repository;

public class UserRepo : IRepository<User>
{
    private readonly string ConnectionString;

    public UserRepo(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public static void InitDb(string connectionString)
    {
        //Creates a connection string
        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        //To connect to db
        string? db = builder.Database;

        builder.Remove("Database");
        string connstr = builder.ToString();

        using (IDbConnection connection = new NpgsqlConnection(connstr))
        {
            connection.Open();

            //IDbCommand represents a SQL statement
            using (IDbCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"DROP DATABASE IF EXISTS {db} WITH (force)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"CREATE DATABASE {db}";
                cmd.ExecuteNonQuery();
            }
            //Changes location to db
            connection.ChangeDatabase(db);

            using (IDbCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS User (
                            userId INT PRIMARY KEY, 
                            username VARCHAR(50) NOT NULL,
                            passwordHash VARCHAR(255) NOT NULL,
                            token VARCHAR(255)
                        )
                    ";
                cmd.ExecuteNonQuery();
            }
        }

    }

    public User Get(int id)
    {
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT *
                                        FROM User
                                        WHERE userId = @id";

                connection.Open();

                var userId = command.CreateParameter();
                userId.DbType = DbType.Int32;
                userId.ParameterName = "id";
                userId.Value = id;
                command.Parameters.Add(userId);

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3)
                        );
                    }
                }
            }
        }
        return null;
    }

    public IEnumerable<User> GetAll()
    {
        List<User> data = new List<User>();
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"SELECT *
                                        FROM User";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new User(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3)
                        ));
                    }
                }
            }
        }
        return data;
    }

    public void Add(User u)
    {
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"INSERT INTO playgroundpoints (userId, username, passwordHash, token)
                                            VALUES (@id, @uname, @pass, @token)";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "id";
                parameter.Value = u.UserId;
                command.Parameters.Add(parameter); 

                parameter.ParameterName = "uname";
                parameter.Value = u.Username;
                command.Parameters.Add(parameter);

                parameter.ParameterName = "pass";
                parameter.Value = u.PasswordHash;
                command.Parameters.Add(parameter);

                parameter.ParameterName = "token";
                

                if (u.Token!=null)
                {
                    parameter.Value = u.Token;
                }
                else
                {
                    parameter.Value = null;
                }
                command.Parameters.Add(parameter);
                command.ExecuteNonQuery();
            }
        }
    }

    public void Update(User u, string[] parameters)
    {
        u.UserId = int.Parse(parameters[0] ?? throw new ArgumentNullException("Id cannot be null"));
        u.Username = parameters[1];
        u.PasswordHash = parameters[2];

        if (parameters[3] != null)
        {
            u.Token = u.Token = parameters[3];
        }
        else
        {
            u.Token = null!;
        }

        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"UPDATE User
                                            SET username = @uname, passwordHash = @pass, token = @token
                                            WHERE userId = @id";

                command.CommandText = @"INSERT INTO playgroundpoints (userId, username, passwordHash, token)
                                            VALUES (@id, @uname, @pass, @token)";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "id";
                parameter.Value = u.UserId;
                command.Parameters.Add(parameter);

                parameter.ParameterName = "uname";
                parameter.Value = u.Username;
                command.Parameters.Add(parameter);

                parameter.ParameterName = "pass";
                parameter.Value = u.PasswordHash;
                command.Parameters.Add(parameter);

                parameter.ParameterName = "token";
                parameter.Value = u.Token;
                command.Parameters.Add(parameter);
                command.ExecuteNonQuery();
            }
        };
    }

    public void Delete(User u)
    {
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"DELETE FROM User WHERE userId = @id";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "id";
                parameter.Value = u.UserId;
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }
        }
    }
}