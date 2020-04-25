using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Resader.Host.Filters;
using Resader.Host.Grains;

namespace Resader.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(config["Urls"].Split(';').ToArray());
                    webBuilder.UseStartup<Startup>();
                })
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "resader";
                        options.ServiceId = "resader";
                    })
                    .AddIncomingGrainCallFilter<ExceptionFilter>()
                    .UseAdoNetClustering(options =>
                    {
                        options.Invariant = "MySql.Data.MySqlClient";
                        options.ConnectionString = config["MySql:ConnectionString"];
                    })
                    .UseAdoNetReminderService(options => 
                    {
                        options.ConnectionString = config["MySql:ConnectionString"];
                        options.Invariant = "MySql.Data.MySqlClient";
                    })
                    .ConfigureApplicationParts(parts =>
                    {
                        parts.AddApplicationPart(typeof(UserGrain).Assembly)
                            .AddApplicationPart(typeof(RssGrain).Assembly)
                            .AddApplicationPart(typeof(FetcherGrain).Assembly)
                            .WithReferences();
                    })
                    .ConfigureEndpoints(siloPort: 7854, gatewayPort: 5489);
                });
        }
    }
}
