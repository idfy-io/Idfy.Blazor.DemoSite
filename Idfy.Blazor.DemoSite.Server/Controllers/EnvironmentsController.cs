using Idfy.Blazor.DemoSite.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Idfy.Blazor.DemoSite.Server.Controllers
{
    [Route("api/[controller]")]
    public class EnvironmentsController : Controller
    {
        private readonly AppSettings appSettings;

        public EnvironmentsController(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            return Ok(appSettings.Environments.Select(e => e.Key));           
        }
    }
}
