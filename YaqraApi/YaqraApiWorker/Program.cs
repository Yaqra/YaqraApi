using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YaqraApiWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();

            builder.Services.AddHostedService(serviceProvider => 
                new Worker(serviceProvider.GetService<ILogger<Worker>>(), serviceProvider.GetService<IConfiguration>()));

            var host = builder.Build();
            host.Run();
        }
    }
}