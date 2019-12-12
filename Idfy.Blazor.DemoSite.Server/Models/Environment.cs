namespace Idfy.Blazor.DemoSite.Server
{
    public class IdfyEnvironment
    {      
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiBaseUrl { get; set; }
        public string TokenUrl { get; set; }
        public string OauthBaseUrl { get; internal set; }
        public string AddScopes { get; set; }
    }
}
