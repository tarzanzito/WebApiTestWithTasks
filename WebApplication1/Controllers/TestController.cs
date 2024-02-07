using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;

//for versions read
//https://briancaos.wordpress.com/2022/10/14/c-net-swagger-api-versioning-show-versions-in-your-swagger-page/

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        //private readonly ILogger<TestController> _logger;
        private readonly IStatisticsInfo _statisticsInfo;

        private static readonly object _padlock = new();
        private static readonly Random rnd = new();

        //public TestController(ILogger<TestController> logger, IStatisticsInfo statisticsInfo)
        public TestController(IStatisticsInfo statisticsInfo)
        {
            //_logger = logger; //this is NOT serilog !!! is "Microsoft.Extensions.Logging.Logger"

            _statisticsInfo = statisticsInfo;

            lock (_padlock)
            {
                _statisticsInfo.InstancesCount++;
                _statisticsInfo.InstancesActives++;
            }

            LogWrite($"New {nameof(TestController)}: Total:{_statisticsInfo.InstancesCount} Active:{_statisticsInfo.InstancesActives}");
        }

        ~TestController()
        {
            lock (_padlock)
            {
                _statisticsInfo.InstancesActives--;
            }

            LogWrite($"DEStructor: Total:{_statisticsInfo.InstancesCount} Active:{_statisticsInfo.InstancesActives}");
        }



        [HttpGet(template: "GetExample")]
        //public async Task<string> GetExampleAsync() //only return 200 OK
        public async Task<ActionResult<string>> GetExampleAsync() //https://code-maze.com/aspnetcore-web-api-return-types/
        {
            lock (_padlock)
            {
                _statisticsInfo.MethodActives++;
                _statisticsInfo.MethodsCount++;
            }

            string now1 = DateTime.UtcNow.ToString("yyy/MM/ss hh:mm:ss:fff");


            int delay = rnd.Next(1, 10) * 1000;
            await Task.Delay(delay); //operation


            string now2 = DateTime.UtcNow.ToString("yyy/MM/ss hh:mm:ss:fff");

            string msg = $"Started:{now1}, Ended:{now2}, delay:{delay}, TotalInstances:{_statisticsInfo.InstancesCount}, ActiveInstances:{_statisticsInfo.InstancesActives}, MethodsCount:{_statisticsInfo.MethodsCount}, MethodActives:{_statisticsInfo.MethodActives}"; ;

            lock (_padlock)
            {
                //msg = $"Started:{now1}, Ended:{now2}, delay:{delay}, TotalInstances:{_statisticsInfo.InstancesCount}, ActiveInstances:{_statisticsInfo.InstancesActives}, MethodsCount:{_statisticsInfo.MethodsCount}, MethodActives:{_statisticsInfo.MethodActives}";
                _statisticsInfo.MethodActives--;
            }

            LogWrite($"GetExample: {msg}");


            //Is good or bad ?!?!??!
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();


            return msg; //if return is an ActionResult<string>
            //return Ok(msg);//if return is an IActionResult
            //return BadRequest("Name should be between 3 and 30 characters.");
        }

        private static void LogWrite(string msg)
        {
            Log.Information(msg);
        }
    }
}

//lock (_padlock)
//{ }

//OR

// Mutex mutex = new Mutex();
//if (mutex.WaitOne())
//{
//    //....
//    mutex.ReleaseMutex();
//}


//Mutex mutex = new Mutex();
//if (mutex.WaitOne(5000)) //espera 5 segundos se nao acabar em 5 segundo sai por false
//{
//   
//    try { }
//          ....
//    catch { }
//    finally { mutex.ReleaseMutex(); }
//}
//else
//    Console.WriteLine("Mutex has not been released in 5 seconds");
//}
