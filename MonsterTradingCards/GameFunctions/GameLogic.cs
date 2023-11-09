using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.Repository;
using MonsterTradingCards.REST_Interface;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MonsterTradingCards.GameFunctions
{
    public class GameLogic
    {
        /*
        public static List<HashSet<Card>> packages = new List<HashSet<Card>>();
        public static Dictionary<string,List<Card>> userCards = new Dictionary<string, List<Card>>();
        public static Dictionary<string, List<Card>> userDeck = new Dictionary<string, List<Card>>();

        public static void addPackage(HashSet<Card> cards)
        {
            packages.Add(cards);
            foreach (Card card in cards)
            {
                Console.WriteLine(card.ToString());
            }
        }

        public static bool packageExists()
        {
            if (packages.Count() > 0)
            {
                return true;
            }
            return false;
        }

        public static void userAquirePackage(string name,DbRepo dbRepo)
        {
            var cards = (List<Card>)dbRepo.GetCardPackage();
           // var foundCard = cards.FirstOrDefault(dbcard => dbcard.Id == card.Id);
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
                List<Card> CardList = new List<Card>();
                foreach (Card card in packages[0])
                {
                    CardList.Add(card);
                }
                userCards.Add(name,CardList);
            }
            packages.RemoveAt(0);
            
        }

        public static List<Card> userGetCards(string name)
        {
            if (userCards.ContainsKey(name))
            {
                return userCards[name];
            }

            return null;
        }

        public static bool userSelectCards(string name,List<Card> cardIds)
        {
                List<Card> deckList = new List<Card>();
                foreach (Card card in userCards[name])
                {
                    if (card.Id == cardIds[0].Id)
                    {
                        deckList.Add(card);
                        cardIds.RemoveAt(0);
                        if(cardIds.Count == 0)
                        {
                            break;
                        }
                    }
                }
                if(deckList.Count == 4) {
                userDeck.Add(name, deckList);
                    return true;
                }
            return false;
        }

        public static List<Card> userGetDeck(string name)
        {
            if (userDeck.ContainsKey(name))
            {
                return userCards[name];
            }

            return null;
        }*/
        public static void StartBattle(string player1, string player2)
        {
          
        }
    }
}
