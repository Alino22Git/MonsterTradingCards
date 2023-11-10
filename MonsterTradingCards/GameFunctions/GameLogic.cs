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
        public BattleResult StartBattle(List<Card> player1, List<Card> player2)
        {
            BattleResult battleResult = new BattleResult();
            int roundCount = 0;

            while (roundCount < 100 && player1.Count > 0 && player2.Count > 0)
            {
                var cardA = GetRandomCard(player1);
                var cardB = GetRandomCard(player2);

                var roundResult = FightRound(cardA, cardB);
                battleResult.AddRoundResult(roundResult);

                // Update decks based on round result
                UpdateDecks(player1, player2, roundResult);

                roundCount++;
            }

            // Update player stats based on battle result
            UpdatePlayerStats(player1, player2, battleResult);

            return battleResult;
        }

        private Card GetRandomCard(List<Card> deck)
        {
            if (deck.Count == 0)
                return new Card(); // Placeholder for an empty deck

            Random random = new Random();
            int randomIndex = random.Next(0, deck.Count);
            return deck[randomIndex];
        }

        private RoundResult FightRound(Card cardA, Card cardB)
        {
            double damageA = cardA.IsSpell() ? CalculateSpellDamage(cardA) : cardA.Damage;
            double damageB = cardB.IsSpell() ? CalculateSpellDamage(cardB) : cardB.Damage;

            
             if (cardA.IsSpell() && !cardB.IsSpell())
            {
                // Handle Spell vs. Monster fight
                damageA = ApplySpellModifiers(cardA, cardB, damageA);
            }
            else if (!cardA.IsSpell() && cardB.IsSpell())
            {
                // Handle Monster vs. Spell fight
                damageB = ApplySpellModifiers(cardB, cardA, damageB);
            }
            else 
            {
                // Handle Monster vs. Monster fight
                damageA = ApplyMonsterModifiers(cardA, cardB, damageA);
                damageB = ApplyMonsterModifiers(cardB, cardA, damageB);
            }

            return DetermineWinner(cardA, cardB, damageA, damageB);
        }

        private double CalculateSpellDamage(Card cardB)
        {
            throw new NotImplementedException();
        }

        private double ApplyMonsterModifiers(Card cardA, Card cardB, double damageA)
        {
            throw new NotImplementedException();
        }

        private double ApplySpellModifiers(Card cardA, Card cardB, double damageA)
        {
            throw new NotImplementedException();
        }

        private void UpdateDecks(List<Card> deckA, List<Card> deckB, RoundResult roundResult)
        {
            if (roundResult.Winner == Winner.PlayerA)
            {
                deckB.Remove(roundResult.Loser);
                deckA.Add(roundResult.Loser);
            }
            else if (roundResult.Winner == Winner.PlayerB)
            {
                deckA.Remove(roundResult.Loser);
                deckB.Add(roundResult.Loser);
            }
        }

        private void UpdatePlayerStats(List<Card> deckA, List<Card> deckB, BattleResult battleResult)
        {
            // Implement logic to update player stats based on battle result
            // ...
        }
    }

}
