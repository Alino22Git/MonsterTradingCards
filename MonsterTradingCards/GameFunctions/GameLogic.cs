using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.Repository;
using MonsterTradingCards.REST_Interface;
using System.Collections.Generic;

namespace MonsterTradingCards.GameFunctions
{
    internal class GameLogic
    {
       public  List<HashSet<Card>> packages = new List<HashSet<Card>>();


        public void addPackage(HashSet<Card> cards)
        {
            packages.Add(cards);
        }
    }

    
}
