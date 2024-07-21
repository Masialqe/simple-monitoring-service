using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SMS.App.Handlers.Interfaces;
using SMS.App.Services.Interfaces;
using SMS.App.Services.Strategies;
using SMS.App.Handlers;
using SMS.App.Services;
using Polly.Retry;
using NLog.Web;
using Polly;
using NLog;

//Configure logging
var logger = LogManager
            .Setup()
            .LoadConfigurationFromAppSettings()
            .GetCurrentClassLogger();

//Create service
await Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    //services
                    services.AddHostedService<MonitoringService>();
                    services.AddSingleton<IConfigurationService,ConfigurationService>();
                    services.AddSingleton<INotificationService,NotificationService>();

                    //handlers
                    services.AddSingleton<IIntervalCheckingHandler, IntervalCheckingHandler>();
                    services.AddSingleton<INotificationHandler, NotificationHandler>();

                    //strategies
                    services.AddKeyedSingleton<ICheckStrategy, HttpCheckStrategy>("httpStrategy");
                    services.AddKeyedSingleton<ICheckStrategy, PingCheckStrategy>("pingStrategy");

                    //clients
                    services.AddHttpClient<HttpCheckStrategy>()
                    .AddStandardResilienceHandler();

                    //retry policy
                    services.AddResiliencePipeline("default", x =>
                    {
                        x.AddRetry(new RetryStrategyOptions
                        {
                            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                            Delay = TimeSpan.FromSeconds(2),
                            MaxRetryAttempts = 3,
                            BackoffType = DelayBackoffType.Exponential,
                            UseJitter = true
                        })
                        .AddTimeout(TimeSpan.FromSeconds(20));
                    });
                })
                .UseNLog()
                .UseWindowsService()
                .Build()
                .RunAsync();