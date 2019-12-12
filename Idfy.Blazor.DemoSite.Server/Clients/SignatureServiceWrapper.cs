using Idfy.Signature;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Idfy.Blazor.DemoSite.Server.Clients
{
 
    public class SignatureServiceWrapper
    {
        private readonly ConcurrentDictionary<string, SignatureService> environments;
        private readonly ConcurrentDictionary<string, NewFeaturesApiClient> newFeatureClients;
        private readonly AppSettings appSettings;
        private readonly string[] DefaultScopes = new [] { "document_read", "document_write", "document_file" };


        public SignatureServiceWrapper(AppSettings appSettings)
        {
            environments = new ConcurrentDictionary<string, SignatureService>();
            newFeatureClients = new ConcurrentDictionary<string, NewFeaturesApiClient>();          

            foreach (var environment in appSettings.Environments)
            {
                var scopes = DefaultScopes.ToList();

                if(!string.IsNullOrWhiteSpace(environment.Value.AddScopes))
                {
                    scopes.AddRange(environment.Value.AddScopes.Split(' '));
                }

                environments.TryAdd(environment.Key, new SignatureService(environment.Value.ClientId.Trim(), environment.Value.ClientSecret.Trim(), scopes));
                newFeatureClients.TryAdd(environment.Key, new NewFeaturesApiClient(environment.Value.ClientId.Trim(), environment.Value.ClientSecret.Trim(), scopes));
            }
            this.appSettings = appSettings;
        }

        public SignatureService GetService(string environment)
        {
            SignatureService service;
            environments.TryGetValue(environment, out service);

            if (service == null)
                throw new Exception("Unable to find service for current environment");

            return service;
        }

        public INewFeaturesApiClient GetFeaturesApiClient(string environment)
        {
            NewFeaturesApiClient service;
            newFeatureClients.TryGetValue(environment, out service);

            if (service == null)
                throw new Exception("Unable to find newFeatureClient for current environment");

            return service;
        }

        public string SetEnvironment(IHeaderDictionary headers, string fromQuery = null)
        {
            string environmentName = "Test";
            if (headers.TryGetValue("Idfy-Environment", out var result) && result.Count > 0)
            {
                environmentName = result.ToArray()[0];
            }
            else if (!string.IsNullOrWhiteSpace(fromQuery))
                environmentName = fromQuery;

            var environment = appSettings.Environments[environmentName];

            if (!string.IsNullOrWhiteSpace(environment.ApiBaseUrl))
            {
                IdfyConfiguration.BaseUrl = environment.ApiBaseUrl;
            }
            if (!string.IsNullOrWhiteSpace(environment.OauthBaseUrl))
            {
                IdfyConfiguration.OAuthBaseUrl = environment.OauthBaseUrl;
            }
            return environmentName;
        }

    }
}
