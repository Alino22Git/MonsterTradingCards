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
        /// <summary>
        /// Initiates a battle between two users and returns the result as a string.
        /// </summary>
        /// <param name="user1">The first user participating in the battle.</param>
        /// <param name="user2">The second user participating in the battle.</param>
        /// <param name="dbRepo">The database repository for accessing data during the battle.</param>
        /// <returns>A string describing the outcome of the battle.</returns>
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

        /// <summary>
        /// Retrieves a random card from the given deck.
        /// </summary>
        /// <param name="deck">The list of cards from which to retrieve a random card.</param>
        /// <returns>A randomly selected card from the deck.</returns>
        private static Card? GetRandomCard(List<Card?>? deck)
        {
            if (deck!.Count == 0)
                return new Card(); // Placeholder for an empty deck

            Random random = new Random();
            int randomIndex = random.Next(0, deck.Count);
            return deck[randomIndex];
        }

        /// <summary>
        /// Simulates a round of battle between two cards and returns the result.
        /// </summary>
        /// <param name="cardA">The card of the first participant in the battle.</param>
        /// <param name="cardB">The card of the second participant in the battle.</param>
        /// <returns>The result of the battle round.</returns>
        private static RoundResult FightRound(Card? cardA, Card? cardB)
        {
            cardA!.Damage = cardA.IsSpell() ? CalculateSpellDamage(cardA, cardB) : cardA.Damage; //Is the card a spell -> calculate damage against other card
            cardB!.Damage = cardB.IsSpell() ? CalculateSpellDamage(cardB, cardA) : cardB.Damage;

            
            var damageA = SpecialCalculation(cardA, cardB);
            var damageB = SpecialCalculation(cardB, cardA);
            return DetermineWinner(cardA, cardB, damageA, damageB);
        }

        /// <summary>
        /// Determines the winner of a battle round based on the cards and damage dealt.
        /// </summary>
        /// <param name="cardA">The card of the first participant in the battle.</param>
        /// <param name="cardB">The card of the second participant in the battle.</param>
        /// <param name="damageA">The damage dealt by the first participant's card.</param>
        /// <param name="damageB">The damage dealt by the second participant's card.</param>
        /// <returns>The result of the battle round indicating the winner.</returns>
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

        /// <summary>
        /// Calculates the damage dealt by a spell card to an opponent card.
        /// </summary>
        /// <param name="spellCard">The spell card used to attack.</param>
        /// <param name="opponentCard">The opponent's card receiving the attack.</param>
        /// <returns>The calculated damage inflicted by the spell card.</returns>
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

        /// <summary>
        /// Performs special calculations for the battle based on the characteristics of the cards.
        /// </summary>
        /// <param name="card">The card of the participant performing the special calculation.</param>
        /// <param name="opponentCard">The opponent's card involved in the special calculation.</param>
        /// <returns>The result of the special calculation.</returns>
        private static double SpecialCalculation(Card? card, Card? opponentCard)
        {
            string cardType = GetTypeFromCardName(card?.Name);
            string opponentType = GetTypeFromCardName(opponentCard?.Name);
            string opponentElement = GetElementFromCardName(opponentCard?.Name);

            switch (cardType)
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
                case "Spell":
                    if (opponentType == "Kraken")
                    {
                        return 0;
                    }
                    else
                    {
                        return card!.Damage;
                    }
                case "Dragon":
                    if (opponentType == "Elv" && opponentElement=="Fire")
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

        /// <summary>
        /// Retrieves the element type from the given card name.
        /// </summary>
        /// <param name="cardName">The name of the card from which to extract the element.</param>
        /// <returns>The element type extracted from the card name.</returns>
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

            return "Regular";
        }

        /// <summary>
        /// Retrieves the card type from the given card name.
        /// </summary>
        /// <param name="cardName">The name of the card from which to extract the type.</param>
        /// <returns>The card type extracted from the card name.</returns>
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

        /// <summary>
        /// Updates the decks of both players based on the result of a battle round.
        /// </summary>
        /// <param name="deckA">The deck of the first player.</param>
        /// <param name="deckB">The deck of the second player.</param>
        /// <param name="roundResult">The result of the battle round affecting the decks.</param>
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

        /// <summary>
        /// Updates the statistics of both players based on the overall battle result.
        /// </summary>
        /// <param name="player1">The first player participating in the battle.</param>
        /// <param name="player2">The second player participating in the battle.</param>
        /// <param name="battleResult">The overall result of the battle.</param>
        /// <param name="dbRepo">The database repository for updating player statistics.</param>
        /// <param name="log">The log object for recording battle events.</param>
        private static void UpdatePlayerStats(User? player1,User? player2, BattleResult battleResult, DbRepo dbRepo, Log log)
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
