using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using MonsterTradingCards.BasicClasses;
using Npgsql;
using NpgsqlTypes;


namespace MonsterTradingCards.Repository;

public class CardRepo : IRepository<Card>
    {
        private readonly string ConnectionString;

        public CardRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public IEnumerable<Card> GetAll()
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
                        int damage = reader.GetInt32(2);

                        data.Add(new Card(cardId, name, damage));
                    }
                }
            }
        }

        return data;
    }


        public void Update(Card c)
        {
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"UPDATE Card
                                            SET name = @name, damage = @damage
                                            WHERE userId = @id";


                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "id";
                idParameter.Value = c.Id;
                command.Parameters.Add(idParameter);

                var nameParameter = command.CreateParameter();
                nameParameter.ParameterName = "name";
                nameParameter.Value = c.Name;

                var damageParameter = command.CreateParameter();
                damageParameter.ParameterName = "damage";
                damageParameter.Value = c.Damage;
                command.Parameters.Add(damageParameter);

                command.ExecuteNonQuery();
            }
        }
        }

        public void Delete(Card c)
        {
        using (IDbConnection connection = new NpgsqlConnection(ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"DELETE FROM Card WHERE userId = @id";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "id";
                parameter.Value = c.Id;
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }
        }
    }

        public void InitDb(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);

            var db = builder.Database;
            builder.Remove("Database");
            var connstr = builder.ToString();

            using (IDbConnection connection = new NpgsqlConnection(connstr))
            {
                connection.Open();
                connection.ChangeDatabase(db);

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Card (
                            cardId VARCHAR(50) PRIMARY KEY, 
                            name VARCHAR(50) NOT NULL,
                            damage INT
                        )
                    ";
                    cmd.ExecuteNonQuery();
                }
            }
        }
}

