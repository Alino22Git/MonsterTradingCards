namespace MonsterTradingCards.BasicClasses
{
    internal class Spell : Card
    {
        public Spell(string name, int damage, string element)
        {
            this.CardName = name;
            this.Damage = damage;
            this.Element = element;
        }
    }
}
