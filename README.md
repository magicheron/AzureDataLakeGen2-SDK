# AzureDataLakeGen2-SDK
Simple (Unofficial) Azure DataLake Gen 2 SDK implementation

![DataLakeLogo](https://s3-us-west-1.amazonaws.com/striim-prod-media/wp-content/uploads/2018/06/22135502/Azure-DataLake-icon.png)

# Usage

```cs
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

  var stream = new MemoryStream();

  var isFileDownloaded = await client.DownloadFileAsync(filesystem, $"{mypath}/{fileName}", stream);

  if (isFileDownloaded.IsSuccessStatusCode)
  {
      var contentString = UTF8Encoding.UTF8.GetString(stream.ToArray());

      Console.WriteLine(contentString);
  }

  var isFileDeleted = await client.DeleteFileOrDirectoryAsync(filesystem, mypath, true);

  var isFileSystemDeleted = await client.DeleteFilesystemAsync(filesystem);
```

# Azure Data Lake Gen2
This project provides a simple (Unofficial) sdk of Azure Data Lake Gen2, using the [REST APIs](https://docs.microsoft.com/en-us/rest/api/storageservices/datalakestoragegen2/filesystem) for Azure Data Lake Gen2 to create file systems, folder paths and creating and uploading files. 

## Getting started
If you don't have one already, you will need to create an Azure Data Lake Gen2 account by following the instuctions here [https://docs.microsoft.com/en-us/azure/storage/blobs/data-lake-storage-quickstart-create-account](https://docs.microsoft.com/en-us/azure/storage/blobs/data-lake-storage-quickstart-create-account)


Next, you will need to create an Azure Active Directory (AAD) application that will provide the authentication mechanism for the REST calls. The instructions for this are here: 
[https://docs.microsoft.com/en-us/previous-versions/azure/storage/data-lake-storage-rest-api-guide#register-with-azure-active-directory-tenant](https://docs.microsoft.com/en-us/previous-versions/azure/storage/data-lake-storage-rest-api-guide#register-with-azure-active-directory-tenant)

Once you have the AAD application, you need to gather the Application (client) ID, Directory (tenant) ID and Client secret for that application and add them to the web.config appSettings applicationId, tenantID and secretKey respectively.
