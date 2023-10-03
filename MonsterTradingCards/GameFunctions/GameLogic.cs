using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.Repository;
using MonsterTradingCards.REST_Interface;

namespace MonsterTradingCards.GameFunctions
{  
    internal class GameLogic
    {
        private const string DbConnectionString = "Host=localhost;Username=postgres;Password=1223;Database=MonsterTradingCardGame";
        public void StartServer()
        {
            var userRepo = new UserRepo(DbConnectionString);
            UserRepo.InitDb(DbConnectionString);
            var server = new Server(DbConnectionString);
            server.RunServer();
        }



        /*
        private static List<User> ReadDataFromDatabase(int searchObjectId)
        {
            var repo = new UserRepo(DbConnectionString);
            var data = new List<User>();

            if (searchObjectId > 0)
                data.Add(repo.Get(searchObjectId));
            else
                data.AddRange(repo.GetAll());

            return data;
        }

        private static void WriteDataToDatabase(IEnumerable<User> data)
        {
            var repo = new UserRepo(DbConnectionString);
            foreach (var item in data)
            {
                Console.WriteLine("  save item: " + item);
                repo.Add(item);
            }
        }
        */

    }
        
}
