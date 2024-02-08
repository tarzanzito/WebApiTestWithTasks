
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Serilog;
using Serilog.Core;
using Microsoft.AspNetCore.Builder;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = ConfigSerilog("App.log");

            try
            {
                Log.Information("Program Started...");

                var builder = WebApplication.CreateBuilder(args);

                ClearLoggingProviders(builder);
 
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddSingleton<IStatisticsInfo, StatisticsInfo>();

                //

                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                // app.Run();
                AppRun(app);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Main error...");
            }

            Log.Information("Program Finished...");

            Log.CloseAndFlush();
        }

        private static void AppRun(WebApplication app)
        {
            //REF1: if using the "builder.Logging.AddConsole();" 
            //is only need "app.Run();"
            //             "return;"

            app.Start();

            //Next information is automatic send to console only when using 'builder.Logging.AddConsole();'
            //Next information is only available after "app.Start();" 
            string listenIn = "";
            var server = app.Services.GetRequiredService<IServer>();
            var addressFeature = server.Features.Get<IServerAddressesFeature>();

            if (addressFeature != null)
            {
                foreach (var address in addressFeature.Addresses)
                    listenIn += (address + " ");
            }

            Log.Warning($"Now listening on: {listenIn}");
            Log.Warning("Application started. Press Ctrl+C to shut down.");
            Log.Warning($"Hosting environment: {app.Environment.EnvironmentName}");
            Log.Warning($"Content root path: {app.Environment.ContentRootPath}");


            app.WaitForShutdown();
        }

        private static Logger ConfigSerilog(string fileName)
        {
            //To read from "appsettings.json"
            //Package; Serilog.Settings.Configuration  
            //var configuration = new ConfigurationBuilder() //REF2
            //    .AddJsonFile("appsettings.json")
            //    .Build();


            if (File.Exists(fileName))
                File.Delete(fileName);

            Logger log = new LoggerConfiguration()
                //.ReadFrom.Configuration(configuration) //REF2
                //else
                //.WriteTo.File(fileName)// , restrictedToMinimumLevel: LogEventLevel.Debug, rollingInterval: RollingInterval.Day)
                .WriteTo.File(fileName, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            return log;
        }

        private static void ClearLoggingProviders(WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            //builder.Logging.AddConsole(); //REF1
        }
    }
}
