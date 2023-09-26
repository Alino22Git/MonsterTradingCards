using MonsterTradingCards.BasicClasses;

namespace MonsterTradingCards.GameFunctions
{
    internal class GameLogic
    {
        private const string DbConnectionString =
            "Host=localhost;Username=postgres;Password=changeme;Database=simpledatastore";

        public void StartGame()
        {
            PlaygroundPointSqlRepository.InitDb(DbConnectionString);
        }

        private static User ReadDataFromDatabase(int searchObjectId)
        {
            var repo = new UserDbSqlRepo(DbConnectionString);
            var data = new List<PlaygroundPoint>();

            if (searchObjectId > 0)
                data.Add(repo.Get(searchObjectId));
            else
                data.AddRange(repo.GetAll());

            return data;
        }

        private static void WriteDataToDatabase(IEnumerable<PlaygroundPoint> data)
        {
            var repo = new PlaygroundPointSqlRepository(DbConnectionString);
            foreach (var item in data)
            {
                Console.WriteLine("  save item: " + item);
                repo.Add(item);
            }
        }
    }
}
