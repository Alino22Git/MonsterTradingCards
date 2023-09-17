using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.Extensions.Configuration;
using MonsterTradingCards.Database; // Stellen Sie sicher, dass dies der richtige Namespace für Ihre MyDbContext-Klasse ist

namespace MonsterTradingCards.Database
{
    public class DbInterface
    {
        private readonly IConfiguration _configuration;

        public DbInterface(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Konfigurieren Sie die Verbindung zur PostgreSQL-Datenbank
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MyDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Fügen Sie weitere Dienste hinzu, die Sie benötigen
        }
    }
}
