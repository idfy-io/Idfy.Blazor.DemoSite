using System.Collections.Generic;
using System.Threading.Tasks;

namespace Idfy.Blazor.DemoSite.Server
{
    public interface INewFeaturesApiClient
    {
        Task Delete(string url);
        Task<T> GetAsync<T>(string url);
        Task<System.IO.Stream> GetFileAsync(string url);
    }

    public class NewFeaturesApiClient : IdfyBaseService, INewFeaturesApiClient
    {
        public NewFeaturesApiClient(string clientId, string clientSecret, IEnumerable<string> scopes) 
            : base(clientId, clientSecret, scopes)
        {

        }

        Task<System.IO.Stream> INewFeaturesApiClient.GetFileAsync(string url)
        {
            return GetFileAsync(url);
        }

        Task INewFeaturesApiClient.Delete(string url)
        {
            return DeleteAsync(url);
        }

        Task<T> INewFeaturesApiClient.GetAsync<T>(string url)
        {
            return GetAsync<T>(url);            
        }
    }
}
