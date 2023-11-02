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
            DbRepo.InitDb(DbConnectionString);
        }
    }

}
