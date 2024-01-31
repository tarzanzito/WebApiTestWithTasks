using Serilog;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace TestWebApi
{

//Visual Studio - run multi programs
//select solution
//right click -> Properties
//Common Properties / Startup Projects
//select multiple and select projects

    internal class Program
    {
        public static void Main()
        //public static async Task Main()
        {
            Console.WriteLine("App Started....Press enter");
            Console.WriteLine("============================");

            Console.ReadKey();

            int totalTasks = 25;// 25;
            int totalLoops = 10; // 10;


            for (int i = 0; i < totalLoops; i++)
            {
                Console.WriteLine("Loop Start:" + i.ToString());
                Console.WriteLine("============================");

                StartTaskArrayAsync(totalTasks); //run at same time
                //await StartTaskArrayAsync(totalTasks);

                Console.WriteLine("============================");
                Console.WriteLine("Loop End:" + i.ToString());
            }


            Console.WriteLine("============================");
            Console.WriteLine("App Finished....Press enter");
            Console.ReadKey();
        }



        //private static async Task StartTaskArrayAsync(int totalTasks)
        private static void StartTaskArrayAsync(int totalTasks)
        {
            Task[] tasks = new Task[totalTasks];

            for (int i = 0; i < totalTasks; i++)
            {
                int j = i; //C# lambdas capture a reference to the variable, not the value of the variable.
                //tasks[i] = new Task(async () => await Action(j) );
                tasks[i] = new Task(() => Action(j));
            }

            foreach (Task task in tasks)
                task.Start();

            Task.WaitAll(tasks);

            //await Task.WhenAll(tasks);
            //await tasks;
            //Task aa = Task.WhenAll(tasks);
            //await aa;
        }
    

        private static string Action(int id)
        {
            //curl - X 'GET' 'http://localhost:5029/api/Test/GetExample' -H 'accept: text/plain'

            Console.WriteLine($"Action INI:{id}");

            var url = $"http://localhost:5029/api/Test/GetExample";
            var parameters = ""; // "?query={query}&apiKey={Consts.SpoonacularKey}&number=5";


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //HttpResponseMessage response = await client.GetAsync(parameters).ConfigureAwait(true);
            Task< HttpResponseMessage> task = client.GetAsync(parameters);
            task.Wait();
            
            HttpResponseMessage response = task.Result;

            string jsonString = "KO";
            if (response.IsSuccessStatusCode)
            {
                Task<string> taskString = response.Content.ReadAsStringAsync();
                taskString.Wait();

                jsonString = taskString.Result;

                //var recipeList = JsonConvert.DeserializeObject<RecipeList>(jsonString);
                //if (recipeList != null)
                //{
                //    recipes.AddRange(recipeList.Recipes);
                //}
            }
            else
                Console.WriteLine($"Action ERROR:{response.StatusCode}");

            Console.WriteLine($"Action END:{id}={jsonString}");
            
            //response.Dispose();
            //client.Dispose();
            //client = null;

            return jsonString;
        }
    }
}
