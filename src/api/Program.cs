using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Resader.Api;
using Serilog;
using Serilog.Events;

Host.CreateDefaultBuilder(args)
    .UseSerilog((context, config) =>
    {
        config.MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext();
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .Build()
    .Run();