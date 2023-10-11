namespace MonsterTradingCards.BasicClasses
{
    public class Card
    {
        protected int id { get; set; }
        protected string name { get; set; }
        protected double damage{ get; set; }

        public Card()
        {
            id = 0;
            name = null;
            damage = -1.0;
        }

        public Card(int id, string name, double damage)
        {
            this.id = id;
            this.name = name;
            this.damage = damage;
        }
    }
}
