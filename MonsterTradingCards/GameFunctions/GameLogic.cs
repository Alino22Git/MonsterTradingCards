using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MonsterTradingCards.GameFunctions
{
    internal class GameLogic
    {
        public void StartGame()
        {
            CreateHostBuilder(args).Build().Run();

        }
    }
}
