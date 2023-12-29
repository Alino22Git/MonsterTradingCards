using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.BasicClasses
{
    public class Trade
    {
        private string Id { get; set; }
        private string CardToTrade { get; set; }
        private string Type { get; set; }
        private int MinimumDamage { get; set; }

        public Trade(string id, string cardToTrade, string type, int minimumDamage)
        {
            Id = id;
            CardToTrade = cardToTrade;
            Type = type;
            MinimumDamage = minimumDamage;
        }
    }
}
