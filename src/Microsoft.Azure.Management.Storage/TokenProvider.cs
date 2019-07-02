using Microsoft.Rest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.Storage
{
    public class TokenProvider : ITokenProvider
    {
        private readonly string _applicationId;
        private readonly string _secretKey;
        private readonly string _tenantId;

        public TokenProvider(string applicationId, string secretKey, string tenantId)
        {
            this._applicationId = applicationId;
            this._secretKey = secretKey;
            this._tenantId = tenantId;
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            var client = new HttpClient();
            var values = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", this._applicationId },
                { "client_secret", this._secretKey },
                { "resource", "https://storage.azure.com" },
                { "scope", "https://storage.azure.com/.default" }
            };

            try
            {
                //POST to get the access token to the Data Lake
                var content = new FormUrlEncodedContent(values);
                var authResponse = await client.PostAsync($"https://login.microsoftonline.com/{this._tenantId}/oauth2/token", content);
                var authString = await authResponse.Content.ReadAsStringAsync();
                var json = JObject.Parse(authString);

                //Add the Bearer token and API version to the HttpClient -- these will be used in all calls
                return new AuthenticationHeaderValue("Bearer", json["access_token"].ToString());
            }
            catch
            {
                throw new HttpOperationException($"Cant Authenticate Application {this._applicationId}");
            }
        }
    }
}
