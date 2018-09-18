using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;

namespace AzureFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([BlobTrigger("test/{name}", Connection = "CONNECTION_STRING")]Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=testsa123jg;AccountKey=CoejD+rmiSOskZMFXYP5MbXZF4Gehv41hE6IAL9xeVj6YsxlTg0rl2KbdUVc45KUCyK0ZJSlbeUaYE2k7ce4Aw==;EndpointSuffix=core.windows.net");
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference("testwritecontainer");
            container.CreateIfNotExists();
            var blob = container.GetBlockBlobReference(name);
            blob.UploadFromStream(myBlob);
        }
    }
}
