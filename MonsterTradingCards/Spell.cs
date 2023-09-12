namespace MonsterTradingCards
{
    internal class Spell:Card
    {
        public Spell(string name, int damage, string element)
        {
            this.name = name;
            this.damage = damage;
            this.element = element;
        }
    }
}
