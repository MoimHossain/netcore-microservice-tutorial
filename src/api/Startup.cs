using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;
using NCoreWebApp.Sagas.Actors;
using NCoreWebApp.Sagas.Services;

namespace NCoreWebApp
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddControllers();

            // Configure Akka.NET
            var akkaConfig = ConfigurationFactory.ParseString(@"
                akka {
                    actor {
                        provider = ""Akka.Actor.LocalActorRefProvider""
                    }
                    loglevel = INFO
                }
            ");

            // Create ActorSystem and register it
            var actorSystem = ActorSystem.Create("SagaSystem", akkaConfig);
            services.AddSingleton(actorSystem);

            // Start the SAGA manager actor
            var sagaManager = actorSystem.ActorOf(SagaManagerActor.Props(), "saga-manager");

            // Register SAGA service
            services.AddSingleton<ISagaService, SagaService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
