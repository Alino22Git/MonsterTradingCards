using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Npgsql;
using NpgsqlTypes;

namespace MonsterTradingCards.Database.Repository
{
    public class CardSqlRepository : IRepository<Card>
    {
        private readonly string _connectionString;

        public static void InitDb(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);

            string dbName = builder.Database;

            builder.Remove("Database");
            string cs = builder.ToString();

            using (IDbConnection connection = new NpgsqlConnection(cs))
            {
                connection.Open();

                using (IDbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"DROP DATABASE IF EXISTS {dbName} WITH (force)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = $"CREATE DATABASE {dbName}";
                    cmd.ExecuteNonQuery();
                }

                connection.ChangeDatabase(dbName);


                using (IDbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Cards (
                            fId VARCHAR(50) NOT NULL,
                            objectId INT PRIMARY KEY, 
                            shape VARCHAR(50) NOT NULL,
                            anlName VARCHAR(50) NOT NULL,
                            bezirk INT,
                            spielplatzDetail VARCHAR(255) NOT NULL,
                            typDetail VARCHAR(255) NOT NULL,
                            seAnnoCadData VARCHAR(255)
                        )
                    ";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public CardSqlRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Card Get(int id)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT fid, objectid, shape, anlname, bezirk, spielplatzdetail, typdetail, seannocaddata
                                        FROM Cards
                                        WHERE objectid = @id";

                    connection.Open();

                    var pFID = command.CreateParameter();
                    pFID.DbType = DbType.Int32;
                    pFID.ParameterName = "id";
                    pFID.Value = id;
                    command.Parameters.Add(pFID);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Card(
                                reader.GetString(0),
                                reader.GetInt32(1),
                                reader.GetString(2),
                                reader.GetString(3),
                                reader.GetInt32(4),
                                reader.GetString(5),
                                reader.GetString(6),
                                reader.GetString(7)
                            );
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<Card> GetAll()
        {
            List<Card> result = new List<Card>();
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"SELECT fid, objectid, shape, anlname, bezirk, spielplatzdetail, typdetail, seannocaddata
                                            FROM Cards";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Card(
                                reader.GetString(0),
                                reader.GetInt32(1),
                                reader.GetString(2),
                                reader.GetString(3),
                                reader.GetNullableInt32(4),
                                reader.GetString(5),
                                reader.GetString(6),
                                reader.GetString(7)
                            ));
                        }
                    }
                }
            }
            return result;
        }

        public void Add(Card point)
        {
            // This is not ideal, connection should stay open to allow a faster batch save mode
            // but for now it is ok
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"INSERT INTO Cards (fid, objectid, shape, anlname, bezirk, spielplatzdetail, typdetail, seannocaddata)
                                            VALUES (@fid, @objectId, @shape, @anlName, @bezirk, @spielplatzDetail, @typDetail, @seAnnoCadData)";

                    command.AddParameterWithValue("fid", DbType.String, point.FId);
                    command.AddParameterWithValue("objectId", DbType.Int32, point.ObjectId);
                    command.AddParameterWithValue("shape", DbType.String, point.Shape);
                    command.AddParameterWithValue("anlName", DbType.String, point.AnlName);

                    // if (point.Bezirk.HasValue)
                    command.AddParameterWithValue("bezirk", DbType.Int32, point.Bezirk);

                    command.AddParameterWithValue("spielplatzDetail", DbType.String, point.SpielplatzDetail);
                    command.AddParameterWithValue("typDetail", DbType.String, point.TypDetail);
                    command.AddParameterWithValue("seAnnoCadData", DbType.String, point.SeAnnoCadData);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(Card Card, string[] parameters)
        {
            Card.FId = parameters[0] ?? throw new ArgumentNullException("fId cannot be null");
            Card.ObjectId = int.Parse(parameters[1] ?? throw new ArgumentNullException("ObjectId cannot be null"));
            Card.Shape = parameters[2];
            Card.AnlName = parameters[3];

            if (parameters[4] != null)
                Card.Bezirk = int.Parse(parameters[4]);
            else
                // just for clarity, as null is the default value anyway
                Card.Bezirk = null;

            Card.SpielplatzDetail = parameters[5] ?? throw new ArgumentNullException("SpielplatzDetail cannot be null");
            Card.TypDetail = parameters[6] ?? throw new ArgumentNullException("TypDetail cannot be null");
            Card.SeAnnoCadData = parameters[7];

            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"UPDATE Cards
                                            SET fid = @fid, shape = @shape, anlname = @anlName, bezirk = @bezirk, 
                                            spielplatzdetail = @spielplatzDetail, typdetail = @typDetail, seannocaddata = @seAnnoCadData
                                            WHERE objectid = @objectId";

                    command.AddParameterWithValue("fid", DbType.String, Card.FId);
                    command.AddParameterWithValue("shape", DbType.String, Card.Shape);
                    command.AddParameterWithValue("anlName", DbType.String, Card.AnlName);
                    command.AddParameterWithValue("bezirk", DbType.Int32, Card.Bezirk);
                    command.AddParameterWithValue("spielplatzDetail", DbType.String, Card.SpielplatzDetail);
                    command.AddParameterWithValue("typDetail", DbType.String, Card.TypDetail);
                    command.AddParameterWithValue("seAnnoCadData", DbType.String, Card.SeAnnoCadData);
                    command.AddParameterWithValue("objectId", DbType.Int32, Card.ObjectId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(Card Card)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"DELETE FROM Cards WHERE objectid = @objectId";

                    command.AddParameterWithValue("objectId", DbType.Int32, Card.ObjectId);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
