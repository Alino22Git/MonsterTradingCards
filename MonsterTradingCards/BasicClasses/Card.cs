namespace MonsterTradingCards.BasicClasses
{
    public class Card
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public double Damage { get; set; }
        public int? Deck { get; set; }

        public Card()
        {
            Id = null;
            Name = null;
            Damage = -1.0;
            Deck = 0;
        }

        public Card(string? id, string? name, double damage, int deck)
        {
            this.Id = id;
            this.Name = name;
            this.Damage = damage;
            this.Deck = deck;
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is Card otherCard)
            {
                // Vergleichen Sie nur das Id-Attribut
                return Id == otherCard.Id;
            }
            return false;
        }

        public override string ToString()
        {
            return "Card: Id: " + Id + " Name: " + Name + " Damage: " + Damage;
        }
    }
}