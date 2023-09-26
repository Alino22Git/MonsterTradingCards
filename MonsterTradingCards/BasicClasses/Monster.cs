namespace MonsterTradingCards.BasicClasses
{
    internal class Monster : Card
    {
        public Monster(string name, int damage, string element) :base ()
        {
            this.CardName = name;
            this.Damage = damage;
            this.Element = element;
        }
    }
}
