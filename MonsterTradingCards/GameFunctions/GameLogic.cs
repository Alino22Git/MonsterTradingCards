using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.Repository;
using MonsterTradingCards.REST_Interface;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace MonsterTradingCards.GameFunctions
{
    public class GameLogic
    {
        public static BattleResult StartBattle(List<Card> deck1, List<Card> deck2)
        {
            BattleResult battleResult = new BattleResult();
            int roundCount = 0;

            while (roundCount < 100 && deck1.Count > 0 && deck2.Count > 0)
            {
                var player1 = GetRandomCard(deck1);
                var player2 = GetRandomCard(deck2);

                var roundResult = FightRound(player1, player2);
                battleResult.AddRoundResult(roundResult);

                UpdateDecks(deck1, deck2, roundResult);

                roundCount++;
            }

            UpdatePlayerStats(deck1, deck2, battleResult);

            return battleResult;
        }

        private static Card GetRandomCard(List<Card> deck)
        {
            if (deck.Count == 0)
                return new Card(); // Placeholder for an empty deck

            Random random = new Random();
            int randomIndex = random.Next(0, deck.Count);
            return deck[randomIndex];
        }

        private static RoundResult FightRound(Card cardA, Card cardB)
        {
            double damageA = cardA.IsSpell() ? CalculateSpellDamage(cardA,cardB) : cardA.Damage;
            double damageB = cardB.IsSpell() ? CalculateSpellDamage(cardB,cardA) : cardB.Damage;

            damageA = SpecialCalculation(cardA, cardB);
            damageB = SpecialCalculation(cardB, cardA);
            return DetermineWinner(cardA, cardB, damageA, damageB);
        }
        

        private static RoundResult DetermineWinner(Card cardA, Card cardB, double damageA, double damageB)
        {
            RoundResult roundResult;
            if (damageA > damageB)
            {
                roundResult = new RoundResult(cardA, cardB, Winner.PlayerA);
            }
            else if (damageB > damageA)
            {
                roundResult = new RoundResult(cardB, cardA, Winner.PlayerB);
            }
            else
            {
                roundResult = new RoundResult(null, null, Winner.Draw);
            }

            return roundResult;
        }

        private static double CalculateSpellDamage(Card spellCard, Card opponentCard)
        {
            string spellElement = GetElementFromCardName(spellCard.Name);
            string opponentElement = GetElementFromCardName(opponentCard.Name);


            switch (spellElement)
            {
                case "Water":
                    if (opponentElement == "Fire")
                    {
                        return spellCard.Damage * 2;
                    }
                    else if (opponentElement == "Regular")
                    {
                        return spellCard.Damage * 0.5;
                    }
                    else
                    {
                        return spellCard.Damage;
                    }
                case "Fire":
                    if (opponentElement == "Regular")
                    {
                        return spellCard.Damage * 2;
                    }
                    else if (opponentElement == "Water")
                    {
                        return spellCard.Damage * 0.5;
                    }
                    else
                    {
                        return spellCard.Damage;
                    }
                case "Regular":
                    if (opponentElement == "Water")
                    {
                        return spellCard.Damage * 2;
                    }
                    else if (opponentElement == "Fire")
                    {
                        return spellCard.Damage * 0.5;
                    }
                    else
                    {
                        return spellCard.Damage;
                    }
                default:
                    return spellCard.Damage; 
            }
        }
        private static double SpecialCalculation(Card card, Card opponentCard)
        {
            string spellType = GetElementFromCardName(card.Name);
            string opponentType = GetElementFromCardName(opponentCard.Name);
            string opponentElement = GetElementFromCardName(opponentCard.Name);

            switch (spellType)
            {
                case "Goblin":
                    if (opponentType == "Dragon")
                    {
                        return 0;
                    }
                    else
                    {
                        return card.Damage;
                    }
                case "Ork":
                    if (opponentType == "Wizard")
                    {
                        return 0;
                    }
                    else
                    {
                        return card.Damage;
                    }
                case "Knight":

                    if (opponentType == "Spell" && opponentElement == "Water")
                    {
                        return 0;
                    }
                    else
                    {
                        return card.Damage;
                    }
                default:
                    return card.Damage;
            }
        }
        private static string GetElementFromCardName(string cardName)
        {
            string[] elements = { "Water", "Fire", "Regular" }; 

            foreach (string element in elements)
            {
                if (cardName.Contains(element, StringComparison.OrdinalIgnoreCase))
                {
                    return element;
                }
            }

            return "Unknown";
        }

        private static string GetTypeFromCardName(string cardName)
        {
            string[] types = { "Knight", "Goblin", "Dragon", "Ork", "Wizard","Elv", "Spell", "Kraken" };

            foreach (string t in types)
            {
                if (cardName.Contains(t, StringComparison.OrdinalIgnoreCase))
                {
                    return t;
                }
            }

            return "Unknown";
        }
        private static void UpdateDecks(List<Card> deckA, List<Card> deckB, RoundResult roundResult)
        {
            if (roundResult.RoundWinner == Winner.PlayerA)
            {
                deckB.Remove(roundResult.Loser);
                deckA.Add(roundResult.Loser);
            }
            else if (roundResult.RoundWinner == Winner.PlayerB)
            {
                deckA.Remove(roundResult.Loser);
                deckB.Add(roundResult.Loser);
            }
        }

        private static void UpdatePlayerStats(List<Card> deckA, List<Card> deckB, BattleResult battleResult)
        {
            
        }
    }

}
