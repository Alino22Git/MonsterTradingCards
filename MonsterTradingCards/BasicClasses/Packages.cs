namespace MonsterTradingCards.BasicClasses
{
    internal class Packages
    {
        private HashSet<Card> package = new HashSet<Card>();

        public Packages()
        {
        }

        public Packages(HashSet<Card> cards)
        {
            package = cards;
        }
    }
}