using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ncore_test1
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                
                string hostName = System.Net.Dns.GetHostName();
                string ipAddress = string.Join(" | ", (await System.Net.Dns.GetHostEntryAsync(hostName)).AddressList.Select(ip=> ip.ToString()));
                var mid = $"Machine: {Environment.MachineName},Host: {hostName} IP: {ipAddress}";

                await context.Response.WriteAsync($" Application B {mid}!");
            });
        }
    }
}
