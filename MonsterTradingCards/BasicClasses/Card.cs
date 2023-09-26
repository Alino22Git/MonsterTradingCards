namespace MonsterTradingCards.BasicClasses
{
    public class Card
    {
        protected string CardName { get; set; }
        protected int Damage { get; set; }
        protected string Element{ get; set; }

        public Card():base()
        {
            CardName = "Null";
            Damage = -1;
            Element = "Null";
        }
        public Card(string name, int damage, string element) : base()
        {
            this.CardName = name;
            this.Damage = damage;
            this.Element = element;
        }


    }
}
