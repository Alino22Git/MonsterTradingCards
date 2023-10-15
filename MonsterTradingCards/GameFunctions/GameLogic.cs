using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.Repository;
using MonsterTradingCards.REST_Interface;
using System.Collections.Generic;

namespace MonsterTradingCards.GameFunctions
{
    internal class GameLogic
    {
       public  static List<HashSet<Card>> packages = new List<HashSet<Card>>();


        public static void addPackage(HashSet<Card> cards)
        {
            packages.Add(cards);
            foreach (Card card in cards)
            {
                Console.WriteLine(card.ToString());
            }

            Console.WriteLine(packages.Count());
        }
    }

    
}
