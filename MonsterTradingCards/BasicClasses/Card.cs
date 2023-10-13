namespace MonsterTradingCards.BasicClasses
{
    public class Card
    {
        protected string? id { get; set; }
        protected string? name { get; set; }
        protected double damage{ get; set; }

        public Card()
        {
            id = null;
            name = null;
            damage = -1.0;
        }

        public Card(string? id, string? name, double damage)
        {
            this.id = id;
            this.name = name;
            this.damage = damage;
        }
    }
}
