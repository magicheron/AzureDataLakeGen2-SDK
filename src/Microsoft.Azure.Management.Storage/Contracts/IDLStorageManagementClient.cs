using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.Storage.Contracts
{
    public interface IDLStorageManagementClient
    {

        ServiceClientCredentials Credentials { get; }

        Task<OperationResult> CreateFilesystemAsync(string filesystem);
    }
}
