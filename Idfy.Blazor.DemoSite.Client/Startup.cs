using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Idfy.Blazor.DemoSite.Client.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Idfy.Blazor.DemoSite.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<HttpClient>();
            services.AddSingleton<DocumentService>();
            services.AddSingleton<EnvironmentService>();
            services.AddStorage();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app")
                ;    
        }
    }
}
