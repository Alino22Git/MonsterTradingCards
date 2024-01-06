using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.GameFunctions;
using MonsterTradingCards.Repository;



namespace UnitTests
{
    public class Tests
    {
        DbRepo dbRepo =
            new DbRepo("Host = localhost; Username = postgres; Password = 1223; Database = MonsterTradingCardGame");

        [SetUp]
        public void Setup()
        {
            DbRepo.InitDb("Host = localhost; Username = postgres; Password = 1223; Database = MonsterTradingCardGame");
            User u1 = new User(1, "Alino22", null, null, null, "123", 0, 0, 0, 0, 0, 0, 0);
            User u2 = new User(2, "Bernd01", null, null, null, "abc", 0, 0, 0, 0, 0, 0, 0);
            dbRepo.AddUserCredentials(u1);
            dbRepo.AddUserCredentials(u2);

            dbRepo.AddCard(new Card("1a", "FireSpell", 20, 0));
            dbRepo.AddCard(new Card("1b", "Ork", 20, 0));
            dbRepo.AddCard(new Card("1c", "Dragon", 20, 0));
            dbRepo.AddCard(new Card("1d", "Knight", 20, 0));
            dbRepo.AddCard(new Card("1e", "Spell", 20, 0));

            dbRepo.UserAquireCards(u1);

            dbRepo.AddCard(new Card("2a", "WaterSpell", 20, 0));
            dbRepo.AddCard(new Card("2b", "WaterGoblin", 20, 0));
            dbRepo.AddCard(new Card("2c", "FireElv", 20, 0));
            dbRepo.AddCard(new Card("2d", "Wizard", 20, 0));
            dbRepo.AddCard(new Card("2e", "WaterElv", 20, 0));

            dbRepo.UserAquireCards(u2);

            List<Card> u1Cards = (List<Card>)dbRepo.UserGetCards(u1)!;
            List<Card> u2Cards = (List<Card>)dbRepo.UserGetCards(u2)!;

            for (int i = 0; i < 4; i++)
            {
                u1Cards[i].Deck = 1;
                dbRepo.UpdateCard(u1Cards[i]);
                u2Cards[i].Deck = 1;
                dbRepo.UpdateCard(u2Cards[i]);
            }
            dbRepo.AddCard(new Card("test1", "Spell", 10, 0));
            List<Card> cards = (List<Card>)dbRepo.GetAllCards();
            foreach (Card card in cards)
            {
                Console.WriteLine(card);
            }
            Console.WriteLine("------------------------------");

            Trade trade = new Trade("1Trade", "1a", "Water", 20,1);
            dbRepo.AddTrade(trade);
        }

        [Test]
        public static void StartBattle_WithEmptyUsers()
        {
            // Arrange
            User user1 = new User();
            User user2 = new User();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => GameLogic.StartBattle(user1, user2, null!));
        }

        [Test]
        public void StartBattle_WithNullDbRepo()
        {
            // Arrange
            List<User> allUsers = (List<User>)dbRepo.GetAllUsers();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => GameLogic.StartBattle(allUsers[0], allUsers[1], null!));
        }

        [Test]
        public void StartBattle_UpdatesPlayerEloCorrectly()
        {
            // Arrange
            List<User> allUsers = (List<User>)dbRepo.GetAllUsers();

            // Act
            string? fightLog = GameLogic.StartBattle(allUsers[0], allUsers[1], dbRepo);

            // Assert
            Assert.That(allUsers[0].Elo, Is.GreaterThanOrEqualTo(0)); // Assuming Elo cannot be negative
            Assert.That(allUsers[1].Elo, Is.GreaterThanOrEqualTo(0)); // Assuming Elo cannot be negative
            Assert.That(allUsers[0].Battles, Is.GreaterThanOrEqualTo(0));
            Assert.That(allUsers[1].Battles, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void StartBattle_ResponsesWithGamelog()
        {
            // Arrange
            List<User> allUsers = (List<User>)dbRepo.GetAllUsers();

            // Act
            string? fightLog = GameLogic.StartBattle(allUsers[0], allUsers[1], dbRepo);

            // Assert
            Assert.That(fightLog, Is.TypeOf<String>());
            Assert.That(fightLog.StartsWith("Gamelog:"));
        }

        [Test]
        public void Db_GetAllUsers()
        {
            // Arrange
            List<User> allUsers = (List<User>)dbRepo.GetAllUsers();

            // Act & Assert
            Assert.That(allUsers.Count == 2);
        }

        [Test]
        public void Db_GetAllCards()
        {
            // Arrange
            List<Card> allCards = (List<Card>)dbRepo.GetAllCards();

            // Act & Assert
            Assert.That(allCards.Count == 11);
        }

        [Test]
        public void Db_GetUserCards()
        {
            // Arrange
            User u1 = dbRepo.GetAllUsers().FirstOrDefault(user => user.UserId == 1) ?? throw new InvalidOperationException();
            List<Card>? u1Cards = (List<Card>)dbRepo.UserGetCards(u1)!;

            // Act & Assert
            Assert.That(u1Cards.Count == 5);
        }

        [Test]
        public void Db_AddUserCredentials()
        {
            // Arrange
            User u3 = new User();

            // Act
            u3.Username = "TestUsername";
            u3.Password = "1234";
            u3.Money = 0;
            u3.Elo = 0;
            u3.Battles = 10;
            dbRepo.AddUserCredentials(u3);
            User updatedU3 = dbRepo.GetAllUsers().FirstOrDefault(user => user.UserId == 3) ?? throw new InvalidOperationException();

            // Assert
            Assert.That(updatedU3.UserId == 3 && updatedU3.Username == "TestUsername" && updatedU3.Password == "1234" &&
                        updatedU3.Money == 20 && updatedU3.Elo == 100 && updatedU3.Battles == 0);
        }

        [Test]
        public void Db_UpdateUser()
        {
            // Arrange
            User u1 = dbRepo.GetAllUsers().FirstOrDefault(user => user.UserId == 1) ?? throw new InvalidOperationException();

            // Act
            if (u1 != null)
            {
                u1.Bio = "Test-Bio";
                u1.Name = "Max Mustermann";
                u1.Image = ":)";

                dbRepo.UpdateUser(u1);
            }

            User updatedU3 = dbRepo.GetAllUsers().FirstOrDefault(user => user.UserId == 1) ?? throw new InvalidOperationException();


            // Assert
            Assert.That(updatedU3.UserId == 1 && updatedU3.Bio == "Test-Bio" && updatedU3.Image == ":)" &&
                        updatedU3.Name == "Max Mustermann");
        }

        [Test]
        public void Db_GetCardPackage_NoCards()
        {
            // Arrange
            // Not Enough Cards in DB

            // Act
            List<Card>? cardPackage = (List<Card>)dbRepo.GetCardPackage()!;

            // Assert
            Assert.That(cardPackage == null);
        }

        [Test]
        public void Db_AddCard()
        {
            // Arrange
            int countBefore = dbRepo.GetAllCards().Count();
            dbRepo.AddCard(new Card("test", "Spell", 10, 0));

            // Act & Assert
            Assert.That(countBefore + 1 == dbRepo.GetAllCards().Count());
        }

        [Test]
        public void Db_UpdateCard()
        {
            // Arrange
            Card card = dbRepo.GetAllCards().FirstOrDefault(card=> card.Id=="test1") ?? throw new InvalidOperationException();
            card.Name = "UltimateDragon";
            card.Damage = 100;
            card.Deck = 1;
            dbRepo.UpdateCard(card);
            Card updatedCard = dbRepo.GetAllCards().FirstOrDefault(card => card.Id == "test1") ?? throw new InvalidOperationException();


            // Act & Assert
            Assert.That(updatedCard.Id == "test1" && updatedCard.Name == "UltimateDragon" && updatedCard.Damage == 100 && updatedCard.Deck == 1 );
        }

        [Test]
        public void Db_GetCardPackage_CorrectWay()
        {
            // Arrange
            dbRepo.AddCard(new Card("test2", "FireWizard", 40, 0));
            dbRepo.AddCard(new Card("test3", "WaterDragon", 60, 0));
            dbRepo.AddCard(new Card("test4", "Goblin", 30, 0));
            dbRepo.AddCard(new Card("test5", "Elv", 40, 0));

            // Act
            List<Card>? cardPackage = (List<Card>)dbRepo.GetCardPackage()!;

            // Assert
            if (cardPackage != null)
            {
                foreach (Card card in cardPackage)
                {
                    Console.WriteLine(card);
                }

                Assert.That(cardPackage.Count() == 5);
            }
        }
        
        [Test]
        public void Db_UserAquireCards_CorrectWay()
        {
            // Arrange
            dbRepo.AddCard(new Card("test2", "FireWizard", 40, 0));
            dbRepo.AddCard(new Card("test3", "WaterDragon", 60, 0));
            dbRepo.AddCard(new Card("test4", "Goblin", 30, 0));
            dbRepo.AddCard(new Card("test5", "Elv", 40, 0));
            User u = new User(3, "Max Mustermann", null, null, null, "1234", 0, 0, 0, 0, 0, 0, 0);
            dbRepo.AddUserCredentials(u);
            User u3 = dbRepo.GetAllUsers().FirstOrDefault(user => user.UserId == 3) ?? throw new InvalidOperationException();

            // Act
            dbRepo.UserAquireCards(u3);
            List<Card>? cardsFromU3 = dbRepo.UserGetCards(u3) as List<Card>;

            // Assert
            if (cardsFromU3 != null)
            {
                foreach (Card card in cardsFromU3)
                {
                    Console.WriteLine(card);
                }

                Assert.That(cardsFromU3.Count == 5);
            }
        }

        [Test]
        public void Db_UserAquireCards_NullArgument()
        {
            //Arrange
            User u = new User();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => dbRepo.UserAquireCards(u));
        }

        [Test]
        public void Db_UpdateUserCardDependency()
        {
            // Arrange
            User u1 = dbRepo.GetAllUsers().FirstOrDefault(user => user.UserId == 1) ?? throw new InvalidOperationException();

            // Act
           dbRepo.UpdateUserCardDependency(1, "2e");
           List<Card>? cards = (List<Card>)dbRepo.UserGetCards(u1)!;
           foreach (Card card in cards)
           {
               Console.WriteLine(card);
           }
            // Assert
            Assert.That((dbRepo.UserGetCards(u1) ?? throw new InvalidOperationException()).Count()==6);
        }

        [Test]
        public void Db_GetAllTrades()
        {
            // Act & Assert
            Assert.That((dbRepo.GetAllTrades() ?? throw new InvalidOperationException()).Count() == 1);
        }

        [Test]
        public void Db_AddTrade()
        {
            // Arrange
            Trade trade = new Trade("2Trade", "1b", "Regular", 20, 1);

            // Act
            dbRepo.AddTrade(trade);

            // Assert
            Assert.That((dbRepo.GetAllTrades() ?? throw new InvalidOperationException()).Count()==2);
        }
        
        [Test]
        public void Db_DeleteTrade()
        {
            // Arrange 
            Trade trade = new Trade("1Trade", "1a", "Water", 20, 1); ;
            
            // Act
            dbRepo.DeleteTrade(trade);

            // Assert
            Assert.That(dbRepo.GetAllTrades()==null);
        }
        

        [Test]
        public void GetTypeFromCardName_TypePlusElement()
        {
            // Arrange
            string name = "FireElv";

            // Act
            string element = GameLogic.GetTypeFromCardName(name);

            // Assert
            Assert.That(element=="Elv");
        }

        [Test]
        public void GetTypeFromCardName_OnlyType()
        {
            // Arrange
            string name = "Wizard";

            // Act
            string element = GameLogic.GetTypeFromCardName(name);

            // Assert
            Assert.That(element == "Wizard");
        }

        [Test]
        public void GetTypeFromCardName_UnknownType()
        {
            // Arrange
            string name = "Demon";

            // Act
            string element = GameLogic.GetTypeFromCardName(name);

            // Assert
            Assert.That(element == "Unknown");
        }

        [Test]
        public void IsSpell_WithSpellInName_ReturnsTrue()
        {
            // Arrange
            var card = new Card { Name = "FireSpell" }; 

            // Act
            bool isSpell = card.IsSpell();

            // Assert
            Assert.IsTrue(isSpell);
        }

        [Test]
        public void IsSpell_WithoutSpellInName_ReturnsFalse()
        {
            // Arrange
            var card = new Card { Name = "Goblin" }; 

            // Act
            bool isSpell = card.IsSpell();

            // Assert
            Assert.IsFalse(isSpell);
        }

        [Test]
        public void IsSpell_WithNullName_ReturnsFalse()
        {
            // Arrange
            var card = new Card { Name = null };

            // Act
            bool isSpell = card.IsSpell();

            // Assert
            Assert.IsFalse(isSpell);
        }

        [Test]
        public void IsSpell_WithEmptyName_ReturnsFalse()
        {
            // Arrange
            var card = new Card { Name = string.Empty };

            // Act
            bool isSpell = card.IsSpell();

            // Assert
            Assert.IsFalse(isSpell);
        }
    }
}