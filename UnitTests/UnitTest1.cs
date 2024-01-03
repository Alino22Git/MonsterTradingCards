using MonsterTradingCards.BasicClasses;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using MonsterTradingCards.GameFunctions;
using MonsterTradingCards.Repository;
using Microsoft.Extensions.Hosting;


namespace UnitTests
{
    public class Tests
    {
        DbRepo dbRepo = new DbRepo("Host = localhost; Username = postgres; Password = 1223; Database = MonsterTradingCardGame");
        [SetUp]
        public void Setup()
        {
            DbRepo.InitDb("Host = localhost; Username = postgres; Password = 1223; Database = MonsterTradingCardGame");
            User u1 = new User(1, "Alino22", null, null, null, "123", 20, 100, 0);
            User u2 = new User(2, "Bernd01", null, null, null, "abc", 20, 100, 0);
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
           
            for (int i=0;i<4;i++)
            {
                u1Cards[i].Deck = 1;
                dbRepo.UpdateCard(u1Cards[i]);
                u2Cards[i].Deck = 1;
               dbRepo.UpdateCard(u2Cards[i]);
            }
            List<Card> cards = (List<Card>)dbRepo.GetAllCards();
            foreach (Card card in cards)
            {
                Console.WriteLine(card);
            }
            
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
            List<User> allUsers=(List<User>)dbRepo.GetAllUsers();

            // Act
            string? fightLog = GameLogic.StartBattle(allUsers[0], allUsers[1], dbRepo);

            // Assert
            Assert.That(allUsers[0].Elo, Is.GreaterThanOrEqualTo(0)); // Assuming Elo cannot be negative
            Assert.That(allUsers[1].Elo, Is.GreaterThanOrEqualTo(0)); // Assuming Elo cannot be negative
            Assert.That(allUsers[0].Battles, Is.GreaterThanOrEqualTo(0));
            Assert.That(allUsers[1].Battles, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void StartBattle_ReturnsBattleLog()
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
        public void StartBattle_ReturnsBattleLog()
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
        public void StartBattle_ReturnsBattleLog()
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
        public void StartBattle_ReturnsBattleLog()
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
        public void StartBattle_ReturnsBattleLog()
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
        public void StartBattle_ReturnsBattleLog()
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
        public void StartBattle_ReturnsBattleLog()
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
        public void StartBattle_ReturnsBattleLog()
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
        public void StartBattle_ReturnsBattleLog()
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
        public void StartBattle_ReturnsBattleLog()
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
        public void StartBattle_ReturnsBattleLog()
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
        public void StartBattle_ReturnsBattleLog()
        {
            // Arrange
            List<User> allUsers = (List<User>)dbRepo.GetAllUsers();

            // Act
            string? fightLog = GameLogic.StartBattle(allUsers[0], allUsers[1], dbRepo);

            // Assert
            Assert.That(fightLog, Is.TypeOf<String>());
            Assert.That(fightLog.StartsWith("Gamelog:"));
        }

    }
}