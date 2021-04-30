using System;
using System.Data;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Polly;
using Resader.Extensions;
using Resader.Factories;
using Resader.Middlewares;
using Resader.Quartz;
using Quartz.Spi;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using StackExchange.Redis;
using Resader.Helpers;
using Resader.Daos;
using Resader.Services;
using Resader.Jobs;

namespace Resader
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
            services.AddControllersWithViews();
            services.AddSwaggerDocument();
            this.AddRateLimit(services);
            services.Configure<Configuration>(this.Configuration);

            services.AddSingleton<DbConnectionFactory>();
            services.AddTransient<IDbConnection>(serviceProvider => AsyncHelper.RunSync<IDbConnection>(() => 
                serviceProvider.GetService<DbConnectionFactory>().CreateDbConnection("MySql", "resader")));
            services.AddSingleton<RedisConnectionFactory>();
            services.AddTransient<UserDao>()
                .AddTransient<RssDao>();
            services.AddHttpClient<FetchService>();

            services.AddCors(o => o.AddPolicy("Default", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            bool.TryParse(this.Configuration["UseScheduler"], out bool useScheduler);
            if (useScheduler)
            {
                services.AddSingleton<IJobFactory, CustomJobFactory>();
                services.AddHostedService<QuartzHostedService>();
                services.AddTransientBothTypes<IScheduledJob, FetchJob>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<Configuration> configuration)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }
            app.UseStaticFiles();

            app.UseMiddleware<LogMiddleware>();

            app.UseIpRateLimiting();

            app.UseCors("Default");
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            Utility.MakeDapperMapping("Resader.Models");
        }

        private void AddRateLimit(IServiceCollection services)
        {
            // needed to load configuration from appsettings.json
            services.AddOptions();

            // needed to store rate limit counters and ip rules
            services.AddMemoryCache();

            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            // inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // https://github.com/aspnet/Hosting/issues/793
            // the IHttpContextAccessor service is not registered by default.
            // the clientId/clientIp resolvers use it.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
