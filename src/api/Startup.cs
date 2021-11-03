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
using Resader.Api.Extensions;
using Resader.Api.Factories;
using Resader.Api.Jobs;
using Resader.Api.Middlewares;
using Resader.Api.Quartz;
using Resader.Api.Services;
using Quartz.Spi;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using StackExchange.Redis;
using Resader.Api.Daos;
using Resader.Api.Helpers;
using System.Net.Http;

namespace Resader.Api
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

            var redisConfig = this.Configuration.GetSection("Redis");
            if (redisConfig?.Value == null)
            {
                services.AddMemoryCache();
                services.AddTransient<ICacheService, MemoryCacheService>();
            }
            else
            {
                services.AddSingleton(_ => ConnectionMultiplexer.Connect(this.Configuration["Redis:ConnectionString"]));
                services.AddTransient(serviceProvider =>
                {
                    int.TryParse(this.Configuration["Redis:DefaultDb"], out int db);
                    return serviceProvider.GetService<ConnectionMultiplexer>().GetDatabase(db);
                });
                services.AddTransient<ICacheService, RedisService>();
            }

            services.AddSingleton<DbConnectionFactory>();
            services.AddTransient<UserDao>()
                .AddTransient<RssDao>()
                .AddTransient<RssService>()
                .AddTransient<UserService>()
                .AddTransient<RecommendDao>()
                .AddTransient<RecommendService>();
            services.AddHttpClient<FetchService>()
                .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                });

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
                services.AddTransientBothTypes<IScheduledJob, AutoRecoveryJob>();
                services.AddTransientBothTypes<IScheduledJob, SaveRecordJob>();
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
            var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);
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

            Utility.MakeDapperMapping("Resader.Common.Entities");
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
