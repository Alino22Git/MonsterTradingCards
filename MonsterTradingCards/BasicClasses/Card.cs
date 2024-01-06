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
        }

        public Card(string? id, string? name, double damage, int deck)
        {
            this.Id = id;
            this.Name = name;
            this.Damage = damage;
            this.Deck = deck;
        }

        public Card(string? id)
        {
            this.Id = id;
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Card otherCard)
            {
                // Vergleichen Sie nur das Id-Attribut
                return Id == otherCard.Id;
            }
            return false;
        }

        public bool IsSpell()
        {
            // Überprüfen Sie, ob der Name "Spell" enthält, um einen Zauber zu identifizieren
            return Name?.IndexOf("Spell", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public override string ToString()
        {
            return "Card: Id: " + Id + " Name: " + Name + " Damage: " + Damage + " Deck: "+ Deck;
        }
    }
}