using System;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Text;
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
using Orleans.Http;
using Resader.Grains;
using Serilog;
using Serilog.Events;

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
            services.AddSingleton<HttpClient>(_ =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                var client = new HttpClient(handler);
                client.Timeout = new TimeSpan(0, 0, 30);
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                return client;
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
            services.AddCors(o => o.AddPolicy("Default", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.Logger(lc =>
                {
                    lc.WriteTo
                        .RollingFile(Path.Combine(this.Configuration["Serilog:BaseFilePath"], "logs/info/{Hour}.txt"))
                        .Filter.ByIncludingOnly(@e => @e.Level == LogEventLevel.Information);
                })
                .WriteTo.Logger(lc =>
                {
                    lc.WriteTo
                        .RollingFile(Path.Combine(this.Configuration["Serilog:BaseFilePath"], "logs/warn/{Hour}.txt"))
                        .Filter.ByIncludingOnly(@e => @e.Level == LogEventLevel.Warning);
                })
                .WriteTo.Logger(lc =>
                {
                    lc.WriteTo
                        .RollingFile(Path.Combine(this.Configuration["Serilog:BaseFilePath"], "logs/error/{Hour}.txt"))
                        .Filter.ByIncludingOnly(@e => @e.Level == LogEventLevel.Error);
                })
                .CreateLogger();

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
                rgppb.RegisterRouteGrainProvider<FixedRouteGrainProvider>(nameof(FixedRouteGrainProvider));
            });

            Utility.MakeDapperMapping("Resader.Host.Models");
        }
    }
}
