namespace MonsterTradingCards.BasicClasses
{
    public class Card
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public double Damage { get; set; }

        public Card()
        {
            Id = null;
            Name = null;
            Damage = -1.0;
        }

        public Card(string? id, string? name, double damage)
        {
            this.Id = id;
            this.Name = name;
            this.Damage = damage;
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