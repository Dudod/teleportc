using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airports.Providers;
using Airports.Providers.AirPortCodes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly;
using Repository;

namespace Distance.Service
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
            services.AddSingleton<IRepository, Repository.Repository>(x => new Repository.Repository("Host=localhost;Port=5432;Database=teleportc;Username=postgres;Password=teleportc"));

            services.AddSingleton<IExternalAirportProvider, AirPortCodesProvider>();
            services.AddSingleton<IAirportsProvider, AirportsProvider>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Distance.Service", Version = "v1" });
            });

            services.AddHttpClient<IExternalAirportProvider, AirPortCodesProvider>(c =>
            {
                c.BaseAddress = new Uri("https://www.air-port-codes.com/api/v1/");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("APC-Auth", "822c15ec4f");
                c.DefaultRequestHeaders.Add("APC-Auth-Secret", "7a16994a3b88ac9");
            })
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1)))
            .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Distance.Service v1"));
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
