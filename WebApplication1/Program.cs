
using Serilog;
using Serilog.Events;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var log = new LoggerConfiguration()
                .WriteTo.File("App.log", restrictedToMinimumLevel: LogEventLevel.Debug, rollingInterval: RollingInterval.Day)
            //.WriteTo.Console()
            .CreateLogger();
            Log.Logger = log;
            
            Log.Information("Program Started...");
          
            var builder = WebApplication.CreateBuilder(args);

            //builder.Logging.ClearProviders();

            builder.Services.AddSingleton<IStatisticsInfo, StatisticsInfo>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();

            Log.Information("Program Finished...");
        }
    }
}
