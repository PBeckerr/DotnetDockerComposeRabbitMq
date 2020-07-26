using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WeatherServiceApi
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureLogging(logging => { logging.AddConsole(); })
                       .ConfigureWebHostDefaults(
                           webBuilder => { webBuilder.UseStartup<Startup>(); }
                       );
        }

        public static async Task Main(string[] args)
        {
            //Give rabbitmq time to start
            Console.WriteLine("Delaying start for rabbitmq");
            await Task.Delay(5000);

            CreateHostBuilder(args)
                .Build()
                .Run();
        }
    }
}