using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.BasicClasses
{
    public class Trade
    {
        public string Id { get; set; }
        public string CardToTrade { get; set; }
        public string Type { get; set; }
        public int MinimumDamage { get; set; }
        public int UserId { get; set; }


        public Trade()
        {

        }

        public Trade(string id, string cardToTrade, string type, int minimumDamage,int userId)
        {
            Id = id;
            CardToTrade = cardToTrade;
            Type = type;
            MinimumDamage = minimumDamage;
            UserId = userId;
        }

        public Trade(string id, string cardToTrade, string type, int minimumDamage)
        {
            Id = id;
            CardToTrade = cardToTrade;
            Type = type;
            MinimumDamage = minimumDamage;
        }
    }
}
