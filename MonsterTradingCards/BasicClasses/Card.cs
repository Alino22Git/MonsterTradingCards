namespace MonsterTradingCards.BasicClasses
{
    internal class Card
    {
        protected string name;
        protected int damage;
        protected string element;

        public Card()
        {
            name = "Null";
            damage = -1;
            element = "Null";
        }
        public Card(string name, int damage, string element)
        {
            this.name = name;
            this.damage = damage;
            this.element = element;
        }


    }
}
