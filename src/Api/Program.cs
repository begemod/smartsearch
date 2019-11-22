using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Api.Common.Constants;
using Api.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Api
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                var webHost = CreateWebHostBuilder(args);

                ConfigureLogging();

                Log.Information("Starting web host");

                await webHost.EnsureIndexCreatedAsync();

                await webHost.RunAsync();

                Log.Information("Web host stopped");

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseStartup<Startup>()
                          .SuppressStatusMessages(true)
                          .UseContentRoot(Directory.GetCurrentDirectory())
                          .ConfigureAppConfiguration(
                            (builderContext, config) =>
                                {
                                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                                    config.AddJsonFile(
                                        $"appsettings.{builderContext.HostingEnvironment.EnvironmentName}.json",
                                        optional: true,
                                        reloadOnChange: true);

                                    if (args?.Length > 0)
                                    {
                                        config.AddCommandLine(args);
                                    }
                                })
                          .UseSerilog()
                          .Build();
        }

        private static void ConfigureLogging()
        {
            var logOutputTemplate = "[{Timestamp:HH:mm:ss:fff} {Level:u3}] [{ThreadId}]" + $"[{{{LogConstants.MediatRRequestType}}}]" + " {Message}{NewLine}{Exception}";

            var logFilePrefix = DateTime.Now.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);

            var loggerConfiguration = new LoggerConfiguration()
                                            .MinimumLevel.Information()
                                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                            .Enrich.WithThreadId()
                                            .Enrich.FromLogContext()
                                            .WriteTo.Console(outputTemplate: logOutputTemplate)
                                            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(le => le.Level == LogEventLevel.Fatal) // write to file only in case of fatal log level
                                                                .WriteTo.File($"Logs/{logFilePrefix}_log.txt", rollingInterval: RollingInterval.Day, outputTemplate: logOutputTemplate));

            Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}
