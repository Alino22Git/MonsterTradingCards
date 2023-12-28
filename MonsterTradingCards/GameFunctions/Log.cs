using MonsterTradingCards.BasicClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace MonsterTradingCards.GameFunctions
{
    public class Log
    {
        public List<String> FightLog { get; }
        private User user1 = null;
        private User user2 = null;

        public Log(User u1, User u2)
        {
            user1 = u1;
            user2 = u2;
            FightLog = new List<String>();
            FightLog.Add("Gamelog:\nFight between: "+user1.Username+" & "+user2.Username);
        }

        public void AddEntry(Card card1,Card card2,RoundResult rs,int roundNum)
        {
            var playerName = "none";
            if (rs.RoundWinner == Winner.Draw)
            {
                FightLog.Add(roundNum + ". Round\n" + card1.Name + " with " + card1.Damage + " damage is fighting against " + card2.Name + " with " + card2.Damage + "\n" + "The Round ends with a draw.\n--------------------");
                return;
            }
            else if (rs.RoundWinner == Winner.PlayerA)
            {
                playerName = user1.Username;
            }
            else
            {
                playerName = user2.Username;
            }
            FightLog.Add(roundNum+". Round\n"+card1.Name +" with "+card1.Damage + " damage is fighting against "+card2.Name + " with " + card2.Damage+"\n"+ playerName + " wins the round.\n--------------------");
        }

        public String GetFightLog()
        {
            var log = String.Join("\n", FightLog);
            Console.WriteLine(log);
            return log;
        }

        public void AddLasEntry(int winsA, int winsB, int draws)
        {
            var winner = "none";
            if (winsA > winsB)
            {
                winner = user1.Username;
            }
            else if(winsA < winsB)
            {
                winner = user2.Username;
            }else
            {
                FightLog.Add("The game lasted: " + (winsA + winsB + draws) + "and ended with a DRAW.");
                return;
            }
           FightLog.Add("The game lasted: "+(winsA+winsB+draws)+"\nThe winner is "+winner+"!\nCONGRATULATIONS");
        }
    }
}

   
