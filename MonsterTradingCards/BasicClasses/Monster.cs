namespace MonsterTradingCards.BasicClasses
{
    internal class Monster : Card
    {
        public Monster(string name, int damage, string element) :base ()
        {
            this.name = name;
            this.damage = damage;
            this.element = element;
        }
    }
}
