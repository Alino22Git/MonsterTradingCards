namespace MonsterTradingCards.BasicClasses
{
    internal class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Token { get; set; }
    }
}