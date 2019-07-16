using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Idfy.Blazor.DemoSite.Client.Services
{
    public class EnvironmentService
    {
        private readonly HttpClient httpClient;
        private string currentEnvironment;

        public string CurrentEnvironment
        {
            get => currentEnvironment;
            set {
                currentEnvironment = value;

                if(httpClient.DefaultRequestHeaders.Contains("Idfy-Environment"))
                {
                    httpClient.DefaultRequestHeaders.Remove("Idfy-Environment");
                }
                httpClient.DefaultRequestHeaders.Add("Idfy-Environment", value);
            }
        }
        public List<string> Environments { get; set; }

        public EnvironmentService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            Environments = new List<string>();
            this.CurrentEnvironment = "Test";
        }

        public async Task Initialize(string baseUrl)
        {
            var result = await httpClient.GetAsync($"{baseUrl}api/Environments");
            var resultAsString = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                this.Environments = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(resultAsString);
            }
            else
            {
                this.Environments = new List<string> { "Test" };
            }

            if (!Environments.Any())
                Environments.Add("Test");
        }
    }
}
