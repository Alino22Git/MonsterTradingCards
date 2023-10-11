namespace MonsterTradingCards.BasicClasses
{
    internal class Packages
    {
        private int id;
        private HashSet<Card> package = new HashSet<Card>();

        public Packages()
        {
        }

        public Packages(int id, HashSet<Card> cards)
        {
            this.id = id;
            package = cards;
        }
    }
}