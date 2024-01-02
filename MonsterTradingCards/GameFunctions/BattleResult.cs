using MonsterTradingCards.BasicClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.GameFunctions
{
    public class BattleResult
    {
        public List<RoundResult> RoundResults { get; }

        public BattleResult()
        {
            RoundResults = new List<RoundResult>();
        }

        public void AddRoundResult(RoundResult roundResult)
        {
            RoundResults.Add(roundResult);
        }

    }

    public class RoundResult
    {
        public Card? Winner { get; }
        public Card? Loser { get; }
        public Winner RoundWinner { get; }

        public RoundResult(Card? winner, Card? loser, Winner roundWinner)
        {
            Winner = winner;
            Loser = loser;
            RoundWinner = roundWinner;
        }
    }

    public enum Winner
    {
        PlayerA,
        PlayerB,
        Draw
    }
}
