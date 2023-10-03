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
      

        static void Main()
        {
            var server = new GameLogic();
            server.StartServer();
        }
    }

}
