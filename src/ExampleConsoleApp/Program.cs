using Microsoft.Azure.Management.Storage;
using Microsoft.Rest;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace ExampleConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Get your AAD Tenant ID, App ID and App Secret to use to aquire token from the app you set up above
            var applicationId = ConfigurationManager.AppSettings["applicationId"];
            var secretKey = ConfigurationManager.AppSettings["secretKey"];
            var tenantId = ConfigurationManager.AppSettings["tenantId"];
            var storageAccountName = "testdatalakegentwo";
            var filesystem = "myfilesystem";
            var mypath = "My/Folder/Path";

            var client = DLStorageManagementClient.CreateClient(applicationId, secretKey, tenantId, storageAccountName);

            var isFileSystemCreated = await client.CreateFilesystemAsync(filesystem);

            var isDirectoryCreated = await client.CreateDirectoryAsync(filesystem, mypath);

            string tmpFile = Path.GetTempFileName();
            string fileName = HttpUtility.UrlEncode(Path.GetFileName(tmpFile));
            File.WriteAllText(tmpFile, $"this is sample file content for {tmpFile}");

            var isFileCreated = await client.CreateFileAsync(filesystem, mypath, fileName, new FileStream(tmpFile, FileMode.Open, FileAccess.Read));

            var isFileDeleted = await client.DeleteFileOrDirectoryAsync(filesystem, mypath, true);

            var isFileSystemDeleted = await client.DeleteFilesystemAsync(filesystem);
        }
    }
}
