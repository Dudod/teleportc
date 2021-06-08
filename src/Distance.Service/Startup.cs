using Airports.Providers;
using Airports.Providers.AirPortCodes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Polly;
using Repository;
using System;

namespace Distance.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddResponseCaching(options =>
            {
                options.UseCaseSensitivePaths = true;
            });

            //Secrets for connection string must be from vault
            services.AddSingleton<IRepository, Repository.Repository>(x => new Repository.Repository(Configuration.GetConnectionString("postgres")));

            services.AddSingleton<IExternalAirportProvider, AirPortCodesProvider>();
            services.AddSingleton<IAirportsProvider, CachedAirportsProvider>();
            services.AddSingleton<IDistanceService, DistanceService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Distance.Service", Version = "v1" });
            });

            services.AddHttpClient<IExternalAirportProvider, AirPortCodesProvider>(c =>
            {
                c.BaseAddress = new Uri("https://www.air-port-codes.com/api/v1/");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("APC-Auth", Configuration.GetValue<string>("AirPortCodes:APC-Auth")); //Secrets must be from vault
                c.DefaultRequestHeaders.Add("APC-Auth-Secret", Configuration.GetValue<string>("AirPortCodes:APC-Auth-Secret")); //Secrets must be from vault
            })
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1)))
            .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Distance.Service v1"));

            app.UseRouting();
            app.UseResponseCaching();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
