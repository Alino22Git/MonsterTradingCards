using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using MonsterTradingCards.Database; // Stellen Sie sicher, dass der richtige Namespace verwendet wird

namespace MonsterTradingCards.REST_Interface
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method is called when the services are configured
        public void ConfigureServices(IServiceCollection services)
        {
            // Fügen Sie PostgreSQL-Datenbankverbindung hinzu
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MyDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Fügen Sie Controller-Dienste hinzu
            services.AddControllers();

            // Hier können Sie weitere Dienste hinzufügen, z.B. für Authentifizierung oder andere Anwendungslogik.
        }

        // This method is called when the application is configured
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // Aktivieren Sie die Endpunkte der Controller
                endpoints.MapControllers();
            });
        }
    }
}

