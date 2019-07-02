using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.Azure.Management.Storage
{
    public class ServicePrincipalTokenCredentials : TokenCredentials
    {
        public ServicePrincipalTokenCredentials(ITokenProvider tokenProvider) : base(tokenProvider)
        {

        }

        public override void InitializeServiceClient<T>(ServiceClient<T> client)
        {
            base.InitializeServiceClient(client);
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            client.HttpClient.DefaultRequestHeaders.Authorization = this.TokenProvider.GetAuthenticationHeaderAsync(token).GetAwaiter().GetResult();
        }
    }
}
