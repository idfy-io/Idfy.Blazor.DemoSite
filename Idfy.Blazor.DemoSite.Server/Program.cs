using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace Idfy.Blazor.DemoSite.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .AddJsonFile("AppSettings/appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"AppSettings/appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: true)
                .Build()
                ;

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .Build()
                ;
        }
    }
}
