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
        public static string? StartBattle(User? user1, User? user2, DbRepo dbRepo)
        {
            if (dbRepo == null)
            {
                throw new ArgumentNullException("Error in StartBattle");
            }
            List<Card?>? deck1 = (List<Card?>)dbRepo.UserGetDeck(user1)!;
            List<Card?>? deck2 = (List<Card?>)dbRepo.UserGetDeck(user2)!;
            BattleResult battleResult = new BattleResult();
            Log fightLog = new Log(user1,user2);
            int roundCount = 0;

            while (roundCount < 100 && deck1!.Count > 0 && deck2!.Count > 0)
            {
                var playerCard1 = GetRandomCard(deck1);
                var playerCard2 = GetRandomCard(deck2);

                var roundResult = FightRound(playerCard1, playerCard2);
                fightLog.AddEntry(playerCard1,playerCard2,roundResult,roundCount+1);
                battleResult.AddRoundResult(roundResult);

                UpdateDecks(deck1, deck2, roundResult);

                roundCount++;
            }

            UpdatePlayerStats(user1, user2, battleResult, dbRepo, fightLog);

            return fightLog.GetFightLog();
        }

        private static Card? GetRandomCard(List<Card?>? deck)
        {
            if (deck!.Count == 0)
                return new Card(); // Placeholder for an empty deck

            Random random = new Random();
            int randomIndex = random.Next(0, deck.Count);
            return deck[randomIndex];
        }

        private static RoundResult FightRound(Card? cardA, Card? cardB)
        {
            cardA!.Damage = cardA.IsSpell() ? CalculateSpellDamage(cardA, cardB) : cardA.Damage;
            cardB!.Damage = cardB.IsSpell() ? CalculateSpellDamage(cardB, cardA) : cardB.Damage;

            
            var damageA = SpecialCalculation(cardA, cardB);
            var damageB = SpecialCalculation(cardB, cardA);
            return DetermineWinner(cardA, cardB, damageA, damageB);
        }


        private static RoundResult DetermineWinner(Card? cardA, Card? cardB, double damageA, double damageB)
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

        private static double CalculateSpellDamage(Card? spellCard, Card? opponentCard)
        {
            string spellElement = GetElementFromCardName(spellCard?.Name);
            string opponentElement = GetElementFromCardName(opponentCard?.Name);


            switch (spellElement)
            {
                case "Water":
                    if (opponentElement == "Fire")
                    {
                        return spellCard!.Damage * 2;
                    }
                    else if (opponentElement == "Regular")
                    {
                        return spellCard!.Damage * 0.5;
                    }
                    else
                    {
                        return spellCard!.Damage;
                    }
                case "Fire":
                    if (opponentElement == "Regular")
                    {
                        return spellCard!.Damage * 2;
                    }
                    else if (opponentElement == "Water")
                    {
                        return spellCard!.Damage * 0.5;
                    }
                    else
                    {
                        return spellCard!.Damage;
                    }
                case "Regular":
                    if (opponentElement == "Water")
                    {
                        return spellCard!.Damage * 2;
                    }
                    else if (opponentElement == "Fire")
                    {
                        return spellCard!.Damage * 0.5;
                    }
                    else
                    {
                        return spellCard!.Damage;
                    }
                default:
                    return spellCard!.Damage;
            }
        }

        private static double SpecialCalculation(Card? card, Card? opponentCard)
        {
            string spellType = GetElementFromCardName(card?.Name);
            string opponentType = GetTypeFromCardName(opponentCard?.Name);
            string opponentElement = GetElementFromCardName(opponentCard?.Name);

            switch (spellType)
            {
                case "Goblin":
                    if (opponentType == "Dragon")
                    {
                        return 0;
                    }
                    else
                    {
                        return card!.Damage;
                    }
                case "Ork":
                    if (opponentType == "Wizard")
                    {
                        return 0;
                    }
                    else
                    {
                        return card!.Damage;
                    }
                case "Knight":

                    if (opponentType == "Spell" && opponentElement == "Water")
                    {
                        return 0;
                    }
                    else
                    {
                        return card!.Damage;
                    }
                default:
                    return card!.Damage;
            }
        }

        private static string GetElementFromCardName(string? cardName)
        {
            string[] elements = { "Water", "Fire", "Regular" };

            foreach (string element in elements)
            {
                if (cardName!.Contains(element, StringComparison.OrdinalIgnoreCase))
                {
                    return element;
                }
            }

            return "Unknown";
        }

        public static string GetTypeFromCardName(string? cardName)
        {
            string[] types = { "Knight", "Goblin", "Dragon", "Ork", "Wizard", "Elv", "Spell", "Kraken" };

            foreach (string t in types)
            {
                if (cardName!.Contains(t, StringComparison.OrdinalIgnoreCase))
                {
                    return t;
                }
            }

            return "Unknown";
        }

        private static void UpdateDecks(List<Card?>? deckA, List<Card?>? deckB, RoundResult roundResult)
        {
            if (roundResult.RoundWinner == Winner.PlayerA)
            {
                deckB?.Remove(roundResult.Loser);
                deckA?.Add(roundResult.Loser);
            }
            else if (roundResult.RoundWinner == Winner.PlayerB)
            {
                deckA?.Remove(roundResult.Loser);
                deckB?.Add(roundResult.Loser);
            }
        }

        private static void UpdatePlayerStats(User? player1,User? player2, BattleResult battleResult,
            DbRepo dbRepo, Log log)
        {
            int winsA = 0, winsB = 0, draws = 0;
            foreach (var round in battleResult.RoundResults)
            {
                if (round.RoundWinner == Winner.PlayerA)
                {
                    winsA++;
                }
                else if (round.RoundWinner == Winner.PlayerB)
                {
                    winsB++;
                }
                else if (round.RoundWinner == Winner.Draw)
                {
                    draws++;
                }
            }

            player1!.Battles = winsA + winsB + draws;
            player1.Elo = player1.Elo + winsA * 3 - winsB * 5;
            player2!.Battles = winsA + winsB + draws;
            player2.Elo = player2.Elo + winsB * 3 - winsA * 5;
            if (player1.Elo < 0)
            {
                player1.Elo = 0;
            }
            else if (player2.Elo < 0)
            {
                player2.Elo = 0;
            }

            log.AddLasEntry(winsA, winsB, draws);
            dbRepo.UpdateUser(player1);
            dbRepo.UpdateUser(player2);
        }
    }
}
