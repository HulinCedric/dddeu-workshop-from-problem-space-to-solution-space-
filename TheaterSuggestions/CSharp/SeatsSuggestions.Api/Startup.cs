using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExternalDependencies;
using ExternalDependencies.AuditoriumLayoutRepository;
using ExternalDependencies.ReservationsProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SeatsSuggestions.Infra;
using Swashbuckle.AspNetCore.Swagger;

namespace SeatsSuggestions.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            IProvideAuditoriumLayouts auditoriumSeatingRepository = new AuditoriumWebRepository("http://localhost:50950/");
            // api/data_for_auditoriumSeating/

            IProvideCurrentReservations seatReservationsProvider = new SeatReservationsWebAdapter("http://localhost:50951/"); 
            // data_for_reservation_seats/

            services.AddSingleton<IProvideAuditoriumLayouts>(auditoriumSeatingRepository);
            services.AddSingleton<IProvideCurrentReservations>(seatReservationsProvider);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "SeatsSuggestions API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseMvc();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SeatsSuggestions API v1");
            });
        }
    }
}
