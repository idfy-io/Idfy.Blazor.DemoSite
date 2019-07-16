using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Idfy.Blazor.DemoSite.Server
{
    public class AppSettings
    {
        public AppSettings(IConfiguration configuration)
        {
            configuration.GetSection("AppSettings").Bind(this);
        }
        
        public Dictionary<string, IdfyEnvironment> Environments { get; set; }
    }
}
