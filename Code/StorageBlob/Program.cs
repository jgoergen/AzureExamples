using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace StorageBlob
{
    class Program
    {
        private static string CONTAINER_NAME = "learningcontainer";
        private static string TEST_BLOCK_BLOB_NAME = "testFile.txt";
        private static string TEST_BLOCK_BLOB_NAME_2 = "testFile2.txt";

        private static CloudStorageAccount storageAccount;
        private static CloudBlobClient blobClient;
        private static CloudBlobContainer blobContainer;

        // TODO: make a demo for 'prefix' matching, pulling back items with XXX in the name

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Azure Storage Blob Examples");
            Console.WriteLine("Connecting to Storage Account Blob");

            // TODO: Run powershell commands to generate the test group and the storage account

            // TODO: Generate the test files and use them instead

            storageAccount =
                CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("StorageConnectionString"));

            blobClient = storageAccount.CreateCloudBlobClient();

            Console.WriteLine($"Connecting to / creating container {CONTAINER_NAME}");

            //  A blob container name must be between 3 and 63 characters in length; start with a letter or number;
            // and contain only letters, numbers, and the hyphen. All letters used in blob container names must be lowercase.
            blobContainer = blobClient.GetContainerReference(CONTAINER_NAME);
            blobContainer.CreateIfNotExists();

            // run tests
            Console.WriteLine("Starting example snippets\r\n");
            UploadFile();
            UploadFileInFolder();
            DownloadFile();
            UploadManyFiles().GetAwaiter().GetResult();
            DownloadManyFile().GetAwaiter().GetResult();
            GetSharedAccessSignatureAccessToken();
            GetStoredAccessPolicyToken();
            ListBlobs();
            CopyBlob();
            CopyBlobAsync();
            CreateDirectory();
            UpdateContainerMetaData();
            Etags();
            LeasingBlobs();

            // add this https://docs.microsoft.com/en-us/azure/storage/blobs/storage-manage-access-to-resources
            // add this https://social.msdn.microsoft.com/Forums/azure/en-US/b41ced45-4d0a-4c81-8dd0-d7d65021fca9/how-to-use-cloudblobcontainerlistblobssegmented-in-new-api?forum=windowsazuredata

            Console.WriteLine($"Examples completed, press enter to quit.");
            Console.ReadLine();
        }

        static void ListBlobs()
        {
            Console.WriteLine($"Starting List Blobs Snippet");
            Console.WriteLine($"Listing all files in container");

            var blobs = blobContainer.ListBlobs();
            foreach(var blob in blobs)
            {
                Console.WriteLine(blob.Uri);
            }

            Console.WriteLine($"Finished List Blobs Snippet\r\n");
        }

        static void LeasingBlobs()
        {
            Console.WriteLine($"Starting Leasing Blobs Snippet");
            Console.WriteLine("Aquiring lease for test text file.");
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(TEST_BLOCK_BLOB_NAME);
            string lease = blockBlob.AcquireLease(TimeSpan.FromSeconds(15), null);
            Console.WriteLine($"Blob lease acquired. Lease = {lease}");

            Console.WriteLine("Updating blob using lease");
            var accessCondition = AccessCondition.GenerateLeaseCondition(lease);
            blockBlob.UploadText("Updated text", accessCondition: accessCondition);
            Console.WriteLine("Blob updated using an exclusive lease");

            //Simulate third party update to blob without lease
            try
            {
                // Below operation will fail as no valid lease provided
                Console.WriteLine("Trying to update blob without valid lease");
                blockBlob.UploadText("Update without lease, will fail");
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed)
                    Console.WriteLine("Precondition failure as expected. Blob's lease does not match");
                else
                    throw;
            }

            Console.WriteLine($"Finished Leasing Blobs Snippet\r\n");
        }

        static void Etags()
        {
            Console.WriteLine($"Starting Etags Snippet");
            Console.WriteLine($"Creating test file");
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference("etagTestFile");
            blockBlob.UploadText("Test upload text");
            var orignalETag = blockBlob.Properties.ETag;
            Console.WriteLine($"Oritinal blob etag {orignalETag}");

            blockBlob.UploadText("A change to this blob");
            Console.WriteLine($"Blob updated, ETag is now {blockBlob.Properties.ETag}");

            try
            {
                Console.WriteLine("Trying to update blob using orignal etag to generate if-match access condition");
                blockBlob.UploadText(
                    "Test upload text",
                    accessCondition:
                        AccessCondition.GenerateIfMatchCondition(orignalETag));
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed)
                {
                    Console.WriteLine("Precondition failure as expected. Blob's orignal etag no longer matches");
                }
                else
                    throw;
            }

            Console.WriteLine($"Finished Etags Snippet\r\n");
        }

        static void CreateDirectory()
        {
            Console.WriteLine($"Starting Create Directory Snippet");
            Console.WriteLine($"Creating Directory references");

            CloudBlobDirectory directory = blobContainer.GetDirectoryReference("test-directory");
            CloudBlobDirectory subDirectory = directory.GetDirectoryReference("sub-directory");

            Console.WriteLine($"Uploading test.txt file to sub directory");

            // Blob name can contain any combination of characters as long as the reserved URL characters are properly escaped.
            // The length of a blob name can range from as short as 1 character to as long as 1024 characters.
            // Avoid blob names that end with a dot (.), a forward slash (/), or a sequence or combination of the two.
            CloudBlockBlob blockBlob = subDirectory.GetBlockBlobReference(TEST_BLOCK_BLOB_NAME);
            using (var fileStream = System.IO.File.OpenRead(@"C:\Users\jeffg\Documents\test.txt"))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            Console.WriteLine($"Finished Create Directory Snippet\r\n");
        }

        static void CopyBlob()
        {
            Console.WriteLine($"Starting Copy Blobs Snippet");
            Console.WriteLine($"Getting test.txt file blob");
            CloudBlockBlob originalBlob = blobContainer.GetBlockBlobReference(TEST_BLOCK_BLOB_NAME);

            Console.WriteLine($"Creating new block blob for copying into");
            CloudBlockBlob newBlob = blobContainer.GetBlockBlobReference("CopiedBlob");
            newBlob.StartCopy(new Uri(originalBlob.Uri.AbsoluteUri));

            Console.WriteLine($"Finished Copy Blob Snippet\r\n");
        }

        static void CopyBlobAsync()
        {
            Console.WriteLine($"Starting Async Copy Blobs Snippet");
            Console.WriteLine($"Getting test.txt file blob");
            CloudBlockBlob originalBlob = blobContainer.GetBlockBlobReference(TEST_BLOCK_BLOB_NAME);

            Console.WriteLine($"Creating new block blob for copying into");
            CloudBlockBlob newBlob = blobContainer.GetBlockBlobReference("CopiedBlob");
            var asyncCallBack = new AsyncCallback(
                x => Console.WriteLine($"*** Async Blob copy completed!"));

            newBlob.BeginStartCopy(
                new Uri(originalBlob.Uri.AbsoluteUri),
                asyncCallBack,
                null);

            Console.WriteLine($"Finished Async Copy Blob Snippet\r\n");
        }

        static void GetSharedAccessSignatureAccessToken()
        {
            Console.WriteLine($"Starting Get Shared Access Token Snippet");

            // This only makes sense if the containers access level is set to private!!
            // Microsoft reccomends you set the start times on these to 15 minutes before your current time to allow for 'clock skew'
            // There will be no references to these generated SAS Tokens in the portal, it cannot be looked up again or revoked
            // Always use HTTPS to create or distribute a SAS. If a SAS is passed over HTTP and intercepted, an attacker performing a
            // man -in-the-middle attack is able to read the SAS and then use it just as the intended user could have,
            // potentially compromising sensitive data or allowing for data corruption by the malicious user.
            // Use near-term expiration times on an ad hoc SAS. In this way, even if a SAS is compromised, it's valid only for a short time.
            // This practice is especially important if you cannot reference a stored access policy. Near-term expiration times also
            // limit the amount of data that can be written to a blob by limiting the time available to upload to it.

            SharedAccessAccountPolicy policy = new SharedAccessAccountPolicy()
            {
                Permissions = SharedAccessAccountPermissions.Write | SharedAccessAccountPermissions.List,
                Services = SharedAccessAccountServices.Blob,
                ResourceTypes = SharedAccessAccountResourceTypes.Object,
                SharedAccessExpiryTime = DateTime.Now.AddMinutes(10),
                Protocols = SharedAccessProtocol.HttpsOnly
            };

            var token = storageAccount.GetSharedAccessSignature(policy);
            Console.WriteLine($"SAS Token retreived: {token}");
            Console.WriteLine($"Finished Get Shared Access Token Snippet\r\n");
        }

        static void GetStoredAccessPolicyToken()
        {
            Console.WriteLine($"Starting Stored Access Policy Access Snippet");

            // This only makes sense if the containers access level is set to private!!
            // Microsoft reccomends you set the start times on these to 15 minutes before your current time to allow for 'clock skew'

            var policyName = "test-policy";

            // Create a new shared access policy and define its constraints.
            // The access policy provides create, write, read, list, and delete permissions.
            // This is unnecessary if you've made one by hand in the portal
            SharedAccessBlobPolicy sharedPolicy = new SharedAccessBlobPolicy()
            {
                // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage
                // service receives the request. Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List
            };

            // Get the container's existing permissions.
            Console.WriteLine($"Getting current container permissions.");
            BlobContainerPermissions permissions = blobContainer.GetPermissions();

            // Add the new policy to the container's permissions, and set the container's permissions.
            Console.WriteLine($"Adding new permissions.");
            permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
            blobContainer.SetPermissions(permissions);

            var token = blobContainer.GetSharedAccessSignature(null, policyName);
            Console.WriteLine($"SAS Token retreived using policy {policyName}: {token}");
            Console.WriteLine($"Finished Stored Access Policy Access Snippet\r\n");
        }

        static void DownloadFile()
        {
            Console.WriteLine($"Starting Download File Snippet");
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(TEST_BLOCK_BLOB_NAME);

            using (var fileStream = System.IO.File.OpenWrite(@"C:\Users\jeffg\Documents\testDownload.txt"))
            {
                blockBlob.DownloadToStream(fileStream);
            }

            Console.WriteLine($"Finished Download File Snippet\r\n");
        }

        static async Task DownloadManyFile()
        {
            // TODO: the files look strange from this, are they right?

            Console.WriteLine($"Starting Download Many Files Snippet");
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(TEST_BLOCK_BLOB_NAME);

            // Define the BlobRequestionOptions on the download, including disabling MD5 hash validation for this example,
            // this improves the download speed.
            BlobRequestOptions options = new BlobRequestOptions
            {
                DisableContentMD5Validation = true,
                StoreBlobContentMD5 = false
            };

            // Retrieve the list of containers in the storage account.
            // Create a directory and configure variables for use later.
            BlobContinuationToken continuationToken = null;
            List<CloudBlobContainer> containers = new List<CloudBlobContainer>();
            do
            {
                var listingResult = await blobClient.ListContainersSegmentedAsync(continuationToken);
                continuationToken = listingResult.ContinuationToken;
                containers.AddRange(listingResult.Results);
            }
            while (continuationToken != null);

            var directory = Directory.CreateDirectory("download");
            BlobResultSegment resultSegment = null;

            // In.NET, the following code increases the default connection limit(which is usually 2 in a client environment
            // or 10 in a server environment) to 100.Typically, you should set the value to approximately the number of threads
            // used by your application.
            // You must set the connection limit before opening any connections.
            ServicePointManager.DefaultConnectionLimit = 20;

            // Download the blobs
            try
            {
                List<Task> tasks = new List<Task>();
                int max_outstanding = 100;
                int completed_count = 0;

                // Create a new instance of the SemaphoreSlim class to define the number of threads to use in the application.
                SemaphoreSlim sem = new SemaphoreSlim(max_outstanding, max_outstanding);

                // Iterate through the containers
                foreach (CloudBlobContainer container in containers)
                {
                    do
                    {
                        // Return the blobs from the container lazily 10 at a time.
                        resultSegment = await container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.All, 10, continuationToken, null, null);
                        continuationToken = resultSegment.ContinuationToken;
                        {
                            foreach (var blobItem in resultSegment.Results)
                            {
                                if (((CloudBlob)blobItem).Properties.BlobType == BlobType.BlockBlob)
                                {
                                    // Get the blob and add a task to download the blob asynchronously from the storage account.
                                    CloudBlockBlob dlBlockBlob = container.GetBlockBlobReference(((CloudBlockBlob)blobItem).Name);
                                    Console.WriteLine("Downloading {0} from container {1}", dlBlockBlob.Name, container.Name);
                                    await sem.WaitAsync();
                                    tasks.Add(dlBlockBlob.DownloadToFileAsync(directory.FullName + "\\" + dlBlockBlob.Name, FileMode.Create, null, options, null).ContinueWith((t) =>
                                    {
                                        sem.Release();
                                        Interlocked.Increment(ref completed_count);
                                    }));

                                }
                            }
                        }
                    }
                    while (continuationToken != null);
                }

                // Creates an asynchronous task that completes when all the downloads complete.
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nError encountered during transfer: {0}", e.Message);
            }


            Console.WriteLine($"Finished Download Many Files Snippet\r\n");
        }

        static void UpdateContainerMetaData()
        {
            Console.WriteLine($"Starting Update Container Meta Data Snippet");
            Console.WriteLine($"Clearing Blob Container Meta Data");
            blobContainer.Metadata.Clear();
            blobContainer.SetMetadata();

            Console.WriteLine($"Getting Blob Container Attributes");
            blobContainer.FetchAttributes();

            Console.WriteLine($"Container url {blobContainer.StorageUri.PrimaryUri.ToString()}");
            Console.WriteLine($"Last modified date {blobContainer.Properties.LastModified.ToString()}");

            // Metadata name/value pairs may contain only ASCII characters.
            // Metadata name/value pairs are valid HTTP headers, and so must adhere to all restrictions governing HTTP headers.
            // It's recommended that you use URL encoding or Base64 encoding for names and values containing non-ASCII characters.
            // The name of your metadata must conform to the naming conventions for C# identifiers.
            Console.WriteLine($"Writting blob container Meta Data");
            blobContainer.Metadata.Add("TestMetaData", $"This is a test {DateTime.Now.ToString()}");
            blobContainer.Metadata["AnotherTest"] = $"More test data {DateTime.Now.ToString()}";
            blobContainer.SetMetadata();

            Console.WriteLine($"Re-Getting updated blob container attributes");
            blobContainer.FetchAttributes();

            Console.WriteLine($"Show all Meta Data");
            foreach (var item in blobContainer.Metadata)
            {
                Console.WriteLine($"{item.Key} = {item.Value}");
            }

            Console.WriteLine($"Finished with Meta Data Snippet\r\n");
        }

        static void UploadFileInFolder()
        {
            Console.WriteLine($"Starting Upload File in Folder Snippet");
            Console.WriteLine($"Uploading test.txt file to blob");

            // Blob name can contain any combination of characters as long as the reserved URL characters are properly escaped.
            // The length of a blob name can range from as short as 1 character to as long as 1024 characters.
            // Avoid blob names that end with a dot (.), a forward slash (/), or a sequence or combination of the two.
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference("test-directory-2/" + TEST_BLOCK_BLOB_NAME);
            using (var fileStream = System.IO.File.OpenRead(@"C:\Users\jeffg\Documents\test.txt"))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            Console.WriteLine($"Finished Upload File in Folder Snippet\r\n");
        }

        static void UploadFile()
        {
            Console.WriteLine($"Starting Upload File Snippet");
            Console.WriteLine($"Uploading test.txt file to blob");

            // Blob name can contain any combination of characters as long as the reserved URL characters are properly escaped.
            // The length of a blob name can range from as short as 1 character to as long as 1024 characters.
            // Avoid blob names that end with a dot (.), a forward slash (/), or a sequence or combination of the two.
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(TEST_BLOCK_BLOB_NAME);
            using (var fileStream = System.IO.File.OpenRead(@"C:\Users\jeffg\Documents\test.txt"))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            Console.WriteLine($"Finished Upload File Snippet\r\n");
        }

        static async Task UploadManyFiles()
        {
            Console.WriteLine($"Starting Upload Many Files Snippet");

            // Blob name can contain any combination of characters as long as the reserved URL characters are properly escaped.
            // The length of a blob name can range from as short as 1 character to as long as 1024 characters.
            // Avoid blob names that end with a dot (.), a forward slash (/), or a sequence or combination of the two.
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(TEST_BLOCK_BLOB_NAME);
            CloudBlockBlob blockBlob2 = blobContainer.GetBlockBlobReference(TEST_BLOCK_BLOB_NAME_2);

            // Define the BlobRequestionOptions on the upload.
            // This includes defining an exponential retry policy to ensure that failed connections are
            // retried with a backoff policy. As multiple large files are being uploaded
            // large block sizes this can cause an issue if an exponential retry policy is not defined.
            // Additionally parallel operations are enabled with a thread count of 2
            // This could be should be multiple of the number of cores that the machine has. Lastly MD5 hash
            // validation is disabled for this example, this improves the upload speed.
            BlobRequestOptions options = new BlobRequestOptions
            {
                ParallelOperationThreadCount = 2,
                DisableContentMD5Validation = true,
                StoreBlobContentMD5 = false
            };

            // In.NET, the following code increases the default connection limit(which is usually 2 in a client environment
            // or 10 in a server environment) to 100.Typically, you should set the value to approximately the number of threads
            // used by your application.
            // You must set the connection limit before opening any connections.
            ServicePointManager.DefaultConnectionLimit = 20;

            List<Task> tasks = new List<Task>();
            tasks.Add(
                blockBlob.UploadFromFileAsync(
                    @"C:\Users\jeffg\Documents\test.txt",
                    null,
                    options,
                    null));

            tasks.Add(
                blockBlob2.UploadFromFileAsync(
                    @"C:\Users\jeffg\Documents\test.txt",
                    null,
                    options,
                    null));

            Console.WriteLine($"Uploading 2 files");
            await Task.WhenAll(tasks);
            Console.WriteLine($"Finished Upload Many Files Snippet\r\n");
        }
    }
}
