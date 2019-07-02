using Microsoft.Azure.Management.Storage.Contracts;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.Storage
{
    public class DLStorageManagementClient : ServiceClient<DLStorageManagementClient>, IDLStorageManagementClient
    {
        public static DLStorageManagementClient CreateClient(string applicationId, string secretKey, string tenantId, string storageAccountName)
        {
            var client = new DLStorageManagementClient(new ServicePrincipalTokenCredentials(new TokenProvider(applicationId, secretKey, tenantId)));
            client.StorageAccountName = storageAccountName;
            client.HttpClient.DefaultRequestHeaders.Add("x-ms-version", "2018-11-09");
            return client;
        }

        /// <summary>
        /// Credentials needed for the client to connect to Azure.
        /// </summary>
        public ServiceClientCredentials Credentials { get; private set; }

        /// <summary>
        /// StorageAccountName needed for the client to connect to Azure Storage Account.
        /// </summary>
        public string StorageAccountName { get; private set; }

        public int Timeout { get; set; }

        private DLStorageManagementClient(ServiceClientCredentials credentials, params DelegatingHandler[] handlers) : base(handlers)
        {
            if (credentials == null)
            {
                throw new System.ArgumentNullException("credentials");
            }
            this.Credentials = credentials;
            if (this.Credentials != null)
            {
                this.Credentials.InitializeServiceClient(this);
            }
            this.Timeout = 10000;
        }

        public async Task<OperationResult> CreateFilesystemAsync(string filesystem)
        {
            var resourceUrl = $"https://{StorageAccountName}.dfs.core.windows.net/{filesystem}?resource=filesystem&timeout={this.Timeout}";
            var response = await this.HttpClient.PutAsync(resourceUrl, null);
            return new OperationResult { IsSuccessStatusCode = response.IsSuccessStatusCode, StatusMessage = response.ReasonPhrase };
        }

        public async Task<OperationResult> DeleteFilesystemAsync(string filesystem)
        {
            var resourceUrl = $"https://{StorageAccountName}.dfs.core.windows.net/{filesystem}?resource=filesystem&timeout={this.Timeout}";
            var response = await this.HttpClient.DeleteAsync(resourceUrl);
            return new OperationResult { IsSuccessStatusCode = response.IsSuccessStatusCode, StatusMessage = response.ReasonPhrase };
        }

        public async Task<OperationResult> CreateDirectoryAsync(string filesystem, string path)
        {
            var resourceUrl = $"https://{StorageAccountName}.dfs.core.windows.net/{filesystem}/{path}?resource=directory&timeout={this.Timeout}";
            var response = await this.HttpClient.PutAsync(resourceUrl, null);
            return new OperationResult { IsSuccessStatusCode = response.IsSuccessStatusCode, StatusMessage = response.ReasonPhrase };
        }

        public async Task<OperationResult> CreateEmptyFileAsync(string filesystem, string path, string fileName)
        {
            var resourceUrl = $"https://{StorageAccountName}.dfs.core.windows.net/{filesystem}/{path}/{fileName}?resource=file&timeout={this.Timeout}";
            using (var tmpContent = new StreamContent(new MemoryStream()))
            {
                HttpRequestMessage newFileMsg = new HttpRequestMessage(HttpMethod.Put, resourceUrl);
                newFileMsg.Content = tmpContent;
                var response = await this.HttpClient.SendAsync(newFileMsg);
                return new OperationResult { IsSuccessStatusCode = response.IsSuccessStatusCode, StatusMessage = response.ReasonPhrase };
            }
        }

        public async Task<OperationResult> CreateFileAsync(string filesystem, string path, string fileName, Stream stream)
        {
            var operationResult = await this.CreateEmptyFileAsync(filesystem, path, fileName);
            if (operationResult.IsSuccessStatusCode)
            {
                using (var streamContent = new StreamContent(stream))
                {
                    //upload to the file buffer
                    var resourceUrl = $"https://{StorageAccountName}.dfs.core.windows.net/{filesystem}/{path}/{fileName}?action=append&timeout={this.Timeout}&position=0";
                    HttpRequestMessage msg = new HttpRequestMessage(new HttpMethod("PATCH"), resourceUrl);
                    msg.Content = streamContent;
                    var response = await this.HttpClient.SendAsync(msg);

                    //flush the buffer to commit the file
                    var flushUrl = $"https://{StorageAccountName}.dfs.core.windows.net/{filesystem}/{path}/{fileName}?action=flush&timeout={this.Timeout}&position={msg.Content.Headers.ContentLength}";
                    HttpRequestMessage flushMsg = new HttpRequestMessage(new HttpMethod("PATCH"), flushUrl);
                    response = await this.HttpClient.SendAsync(flushMsg);

                    return new OperationResult { IsSuccessStatusCode = response.IsSuccessStatusCode, StatusMessage = response.ReasonPhrase };
                }
            } 
            else
            {
                return operationResult;
            }
        }

        public async Task<OperationResult> DeleteFileOrDirectoryAsync(string filesystem, string path, bool recursive = false)
        {
            //delete the file
            var resourceUrl = $"https://{StorageAccountName}.dfs.core.windows.net/{filesystem}/{path}?recursive={recursive}&timeout={this.Timeout}";
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Delete, resourceUrl);
            var response = await this.HttpClient.SendAsync(msg);
            return new OperationResult { IsSuccessStatusCode = response.IsSuccessStatusCode, StatusMessage = response.ReasonPhrase };
        }

    }
}
