
using Serilog;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var log = new LoggerConfiguration().WriteTo.File("App.log")
            //.WriteTo.Console()
            .CreateLogger();

            Log.Logger = log;


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container. 
            builder.Services.AddSingleton<IStaticInfo, StaticInfo>();

            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
