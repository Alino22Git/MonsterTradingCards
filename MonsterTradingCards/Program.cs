using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCards.Classes;
using MonsterTradingCards.GameFunctions;


namespace MonsterTradingCards
{
    internal class Program
    {
        static void Main()
        {
            var gameLogic = new GameLogic();
            gameLogic.StartGame(); 
        }
    }

}
