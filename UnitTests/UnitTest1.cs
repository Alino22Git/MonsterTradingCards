using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.GameFunctions;
using System.Collections.Generic;

namespace UnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            
            HashSet<Card> testcards= new HashSet<Card>();
            testcards.Add(new Card("alsdfjk323","Goblin",20));
            GameLogic.addPackage(testcards);
            if(GameLogic.packages.Count==1)
            Assert.Pass();
        }
    }
}