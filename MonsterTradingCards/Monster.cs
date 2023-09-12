namespace MonsterTradingCards
{
    internal class Monster : Card
    {
        public Monster(string name,int damage, string element)
        {
            this.name = name;
            this.damage = damage;
            this.element = element;
        }
    }
}
