using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Niolog;
using Niolog.Interfaces;
using Orleans.Http;
using Resader.Grains;

namespace Resader.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions()
                .Configure<Configuration>(this.Configuration);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<RssFetcher>();
            services.AddTransient<IDbConnection>(_ =>
            {
                var connection = new MySqlConnection(this.Configuration["MySql:ConnectionString"]);
                return connection;
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["Jwt:Secret"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddAuthorization();

            services.AddGrainRouter()
                .AddJsonMediaType();
            services.AddScheduler();
            services.AddCors(o => o.AddPolicy("Default", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, 
            ILogger<IScheduler> schedulerLogger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            NiologManager.DefaultWriters = new ILogWriter[]
            {
                new ConsoleLogWriter(),
                new FileLogWriter(this.Configuration["Niolog:File"], 10)
            };
            
            loggerFactory.AddProvider(new LoggerProvider());

            var provider = app.ApplicationServices;
            provider.UseScheduler(scheduler =>
            {
                scheduler.Schedule<RssFetcher>()
                    .EveryFiveMinutes()
                    .PreventOverlapping("RssFetcher");
            })
            .LogScheduledTaskProgress(schedulerLogger)
            .OnError(e =>
            {
                var logger = NiologManager.CreateLogger();
                logger.Warn()
                    .Message("Something goes wrong...")
                    .Exception(e, true)
                    .Write();
            });

            var defaultFile = new DefaultFilesOptions();  
            defaultFile.DefaultFileNames.Clear();  
            defaultFile.DefaultFileNames.Add("index.html");  
            app.UseDefaultFiles(defaultFile)
                .UseStaticFiles()
                .UseCookiePolicy()
                .UseCors("Default");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrains("grains");
            });
            app.UseRouteGrainProviders(rgppb =>
            {
                rgppb.RegisterRouteGrainProvider<RandomRouteGrainProvider>(nameof(RandomRouteGrainProvider));
            });

            Utility.MakeDapperMapping("Resader.Host.Models");
        }
    }
}
