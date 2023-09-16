namespace MonsterTradingCards.Classes
{
    internal class Card
    {
        protected string name;
        protected int damage;
        protected string element;

        public Card()
        {
            name = "Null";
            damage = 404;
            element = "None";
        }
        public Card(string name, int damage, string element)
        {
            this.name = name;
            this.damage = damage;
            this.element = element;
        }


    }
}
