using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace PasswordHash
{
    class Program
    {
        static void Main(string[] args)
        {
           var logger = LogManager.GetCurrentClassLogger();
           try
           {
              var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json",
                              optional: true,
                              reloadOnChange: true)
                 .Build();

              var servicesProvider = BuildDi(config);
              using (servicesProvider as IDisposable)
              {
                 var runner = servicesProvider.GetRequiredService<CreateHashedPassword>();
                 runner.DoAction("Action1");

                 //Console.WriteLine("Press ANY key to exit");
                 //Console.ReadKey();
              }
           }
           catch (Exception ex)
           {
              // NLog: catch any exception and log it.
              logger.Error(ex, "Stopped program because of exception");
              throw;
           }
           finally
           {
              // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
              LogManager.Shutdown();
           }
        }



        private static IServiceProvider BuildDi(IConfiguration config)
        {
            return new ServiceCollection()
               .AddTransient<CreateHashedPassword>()
               .AddLogging(builder =>
               {
                   builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                   builder.AddNLog(new NLogProviderOptions
                   {
                       CaptureMessageTemplates = true,
                       CaptureMessageProperties = true
                   });
               })
               //.AddLogging(loggingBuilder =>
               //{
               //// configure Logging with NLog
               //loggingBuilder.ClearProviders();
               //    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
               //    loggingBuilder.AddNLog(config);
               //})
               .BuildServiceProvider();
        }

    }




}
