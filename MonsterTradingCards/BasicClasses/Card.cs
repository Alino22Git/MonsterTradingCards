namespace MonsterTradingCards.BasicClasses
{
    internal class Card
    {
        protected string name { get; set; }
        protected int damage { get; set; }
        protected string element{ get; set; }

        public Card():base()
        {
            name = "Null";
            damage = -1;
            element = "Null";
        }
        public Card(string name, int damage, string element) : base()
        {
            this.name = name;
            this.damage = damage;
            this.element = element;
        }


    }
}
