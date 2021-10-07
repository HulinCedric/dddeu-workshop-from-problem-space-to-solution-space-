using ExternalDependencies;
using ExternalDependencies.AuditoriumLayoutRepository;
using ExternalDependencies.ReservationsProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SeatsSuggestions.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            IProvideAuditoriumLayouts auditoriumSeatingRepository = new AuditoriumLayoutRepository();
            // api/data_for_auditoriumSeating/

            IProvideCurrentReservations seatReservationsProvider = new ReservationsProvider();
            // data_for_reservation_seats/

            services.AddSingleton(auditoriumSeatingRepository);
            services.AddSingleton(seatReservationsProvider);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}