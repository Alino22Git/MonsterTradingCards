using MonsterTradingCards.Repository;
using MonsterTradingCards.REST_Interface;


namespace MonsterTradingCards
{
    internal class Program
    {


        private const string DbConnectionString = "Host=localhost;Username=postgres;Password=1223;Database=MonsterTradingCardGame";
        static void Main()
        {
            createDB();
            
            var server = new Server(DbConnectionString);
            server.RunServer();
        }

        public static void createDB()
        {
            var userRepo = new UserRepo(DbConnectionString);
            UserRepo.InitDb(DbConnectionString);
            var cardRepo = new CardRepo(DbConnectionString);
            cardRepo.InitDb(DbConnectionString);
        }
    }

}
