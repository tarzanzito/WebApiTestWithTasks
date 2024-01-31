using Microsoft.AspNetCore.Mvc;
using Serilog;

//for versions read
//https://briancaos.wordpress.com/2022/10/14/c-net-swagger-api-versioning-show-versions-in-your-swagger-page/

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IStaticInfo _staticInfo;

        private static readonly object _padlock = new object();

        public TestController(ILogger<TestController> logger, IStaticInfo staticInfo)
        {
            _logger = logger; //this is not serilog !!!
            _staticInfo = staticInfo;

            lock (_padlock)
            {
                _staticInfo.InstancesCount++;
                _staticInfo.InstancesActives++;
            }

            LogWrite($"Constructor: Total:{_staticInfo.InstancesCount} Active:{_staticInfo.InstancesActives}");
        }

        ~TestController()
        {
            lock (_padlock)
            {
                _staticInfo.InstancesActives--;
            }

            LogWrite($"DEStructor: Total:{_staticInfo.InstancesCount} Active:{_staticInfo.InstancesActives}");
        }

        [HttpGet(template: "GetExample")]
        public async Task<string> GetExampleAsync() //IActionResult 
        {
            lock (_padlock)
            {
                _staticInfo.MethodActives++;
                _staticInfo.MethodsCount++;
            }

            string now1 = DateTime.UtcNow.ToString("yyy/MM/ss hh:mm:ss:fff");

            await Task.Delay(5000); //operation

            string now2 = DateTime.UtcNow.ToString("yyy/MM/ss hh:mm:ss:fff");

            string msg = "";

            lock (_padlock)
            {
                msg = $"Started:{now1}, Ended:{now2} = Total Instances: {_staticInfo.InstancesCount}, Active Instances: {_staticInfo.InstancesActives}, MethodsCount: {_staticInfo.MethodsCount}, MethodActives:{_staticInfo.MethodActives}";
                _staticInfo.MethodActives--;
            }

            LogWrite($"GetExample: {msg}");


            //Is good or bad ?!?!??!
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            return msg;
        }

        private static void LogWrite(string msg)
        {
            //lock (_padlock)
            {
                Log.Information(msg);
            }
        }

    }
}


// Mutex mutex = new Mutex();
//if (mutex.WaitOne())
//{
//    //....
//    mutex.ReleaseMutex();
//}


//Mutex mutex = new Mutex();
//if (mutex.WaitOne(5000)) //espera 5 segundos se nao acabar em 5 segundo sai por false
//{
//    //....
//    try { }
//    catch { }
//    finally { mutex.ReleaseMutex(); }
//}
//else
//    Console.WriteLine("Mutex has not been released in 5 seconds");

//}
