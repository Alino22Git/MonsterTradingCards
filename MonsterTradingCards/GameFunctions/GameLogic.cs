using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.Repository;
using MonsterTradingCards.REST_Interface;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MonsterTradingCards.GameFunctions
{
    internal class GameLogic
    {
        public static List<HashSet<Card>> packages = new List<HashSet<Card>>();
        public static Dictionary<string,List<Card>> userCards = new Dictionary<string, List<Card>>();


        public static void addPackage(HashSet<Card> cards)
        {
            packages.Add(cards);
            foreach (Card card in cards)
            {
                Console.WriteLine(card.ToString());
            }

            Console.WriteLine(packages.Count());
        }


        public static bool packageExists()
        {
            if (packages.Count() > 0)
            {
                return true;
            }
            return false;
        }

        public static void userAquirePackage(string name)
        {
            if (userCards.ContainsKey(name))
            {
                List<Card> existingCardList = userCards[name];
                foreach (Card card in packages[0])
                {
                    existingCardList.Add(card);
                }
                userCards[name] = existingCardList;
            }
            else
            {
                List<Card> existingCardList = new List<Card>();
                foreach (Card card in packages[0])
                {
                    existingCardList.Add(card);
                }
                userCards.Add(name,existingCardList);
            }
            packages.RemoveAt(0);
            Console.WriteLine(packages.Count());
        }

        public static List<Card> userGetCards(string name)
        {
            if (userCards.ContainsKey(name))
            {
                return userCards[name];
            }

            return null;
        }
    }
}
