using System.Collections.Generic;
using System.Threading.Tasks;

namespace Idfy.Blazor.DemoSite.Server
{
    public interface INewFeaturesApiClient
    {
        Task Delete(string url);
    }

    public class NewFeaturesApiClient : IdfyBaseService, INewFeaturesApiClient
    {
        public NewFeaturesApiClient(string clientId, string clientSecret, IEnumerable<OAuthScope> scopes) 
            : base(clientId, clientSecret, scopes)
        {

        }

        Task INewFeaturesApiClient.Delete(string url)
        {
            return DeleteAsync(url);
        }
    }
}
