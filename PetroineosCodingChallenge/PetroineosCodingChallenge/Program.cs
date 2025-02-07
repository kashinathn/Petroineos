using Microsoft.Extensions.Options;

namespace PetroineosCodingChallenge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Power Position Service starting");

            try
            {
                host.Run();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Service terminated unexpectedly");
            }
            finally
            {
                logger.LogInformation("Power Position Service stopped");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService(config =>
                {
                    config.ServiceName = "PowerPositionService";
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));

                    // Add file logging using community package
                    logging.AddFile(hostContext.Configuration.GetSection("Logging:File"));

                    // Add console logging only in development
                    if (hostContext.HostingEnvironment.IsDevelopment())
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Configuration
                    services.Configure<PowerPositionSettings>(
                        hostContext.Configuration.GetSection(PowerPositionSettings.SectionName));
                    services.AddSingleton(resolver =>
                        resolver.GetRequiredService<IOptions<PowerPositionSettings>>().Value);

                    // Core services
                    services.AddSingleton<ITimeConverter, LondonTimeConverter>();
                    services.AddSingleton<IPowerService, PowerServiceAdapter>();
                    services.AddSingleton<IFileWriter, CsvFileWriter>();
                    services.AddSingleton<PowerPositionCalculator>();

                    // Hosted service
                    services.AddHostedService<PowerPositionWorker>();
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                    options.ValidateOnBuild = true;
                });
    }
}