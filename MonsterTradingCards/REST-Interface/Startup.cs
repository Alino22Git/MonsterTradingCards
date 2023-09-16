namespace MonsterTradingCards.REST_Interface
{
        public class Startup
        {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }

            //This method is called when the services gonna be configured
            public void ConfigureServices(IServiceCollection services)
            {
                //Adds a controller
                services.AddControllers(); 

                // Hier können Sie weitere Dienste hinzufügen, z.B. für Datenbankzugriff oder Authentifizierung!!!!!!
            }

            //This method is called when the application gonna be configured
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    //The endpoints of the controllers gonna be activated
                    endpoints.MapControllers();
                });
            }
        }
    }

