using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.GameFunctions;
using MonsterTradingCards.Repository;
using MonsterTradingCards.REST_Interface;


namespace MonsterTradingCards
{
    internal class Program
    {


        private const string DbConnectionString = "Host=localhost;Username=postgres;Password=1223;Database=MonsterTradingCardGame";
        static void Main()
        {
            var userRepo = new UserRepo(DbConnectionString);
            UserRepo.InitDb(DbConnectionString);
            var server = new Server(DbConnectionString);
            server.RunServer();
        }
    }

}
