using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace StoageQue
{
    class Program
    {
        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=learningstorage123jg;AccountKey=YSYWVeQFVWJmELFzUXFuFX7hYE3KNcChS27ow02Ao6zKl0cbgSqewDl+GwNHTb1NzzY65M82WD6pVBO7jkRVJw==;EndpointSuffix=core.windows.net";
        private const string queName = "testque";

        private static CloudStorageAccount storageAccount;
        private static CloudQueueClient queueClient;
        private static CloudQueue queue;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Azure Storage Que Examples");
            Console.WriteLine("Connecting to Storage Que");

            storageAccount = CloudStorageAccount.Parse(connectionString);
            queueClient = storageAccount.CreateCloudQueueClient();

            // run tests
            Console.WriteLine("Starting example snippets\r\n");
            CreateQueIfNotExists();
            GetEditQueMetaData();
            AddMessageToQue();
            PeekMessage();
            UpdateMessage();
            AddMessageToQue();
            DeleteMessage();
            CreateManyMessages();
            GetQueLength();
            GetAndDeleteManyMessages();
            DeleteQue();

            // cook up a way to demo message poisoning ( set retry limit, then push it over that limit and see what happens. )

            Console.WriteLine($"Examples completed, press enter to quit.");
            Console.ReadLine();
        }

        private static void CreateQueIfNotExists()
        {
            Console.WriteLine("Getting reference to Storage Que");

            // Retrieve a reference to a storage que
            // 3-63 characters long, can contain lowercase letters, numbers, and hyphens
            queue = queueClient.GetQueueReference(queName);

            Console.WriteLine("Creating Storage Que if it doesn't already exist");

            // Create the queue if it doesn't already exist
            if(queue.CreateIfNotExists())
            {
                Console.WriteLine("Que has been created.");
            } else
            {
                Console.WriteLine("Que already exists!");
            }

            Console.WriteLine($"Finished Create Que if not exist Snippet\r\n");
        }

        private static void AddMessageToQue()
        {
            Console.WriteLine("Adding message to Storage Que");

            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage("Hello, World");
            queue.AddMessage(message);

            Console.WriteLine($"Finished Add Message to Que Snippet\r\n");
        }

        private static void GetEditQueMetaData()
        {
            Console.WriteLine("Adding Meta Data to Storage Que");

            // Fetch the queue attributes.
            queue.FetchAttributes();

            // Retrieve the cached approximate message count.
            queue.Metadata.Add($"TestMetaData", $"This is a test {DateTime.Now.ToString()}");
            queue.Metadata["AnotherTest"] = $"More test data {DateTime.Now.ToString()}";
            queue.SetMetadata();

            Console.WriteLine($"Re-Getting updated blob container attributes");
            queue.FetchAttributes();

            Console.WriteLine($"Show all Meta Data");
            foreach (var item in queue.Metadata)
            {
                Console.WriteLine($"{item.Key} = {item.Value}");
            }

            Console.WriteLine($"Finished Adding Meta Data to Que Snippet\r\n");
        }

        private static void PeekMessage()
        {
            Console.WriteLine("Peeking message from Storage Que");

            // Peek at the next message
            CloudQueueMessage peekedMessage = queue.PeekMessage();

            // Display message.
            Console.WriteLine(peekedMessage.AsString);

            Console.WriteLine($"Finished Peek Message Snippet\r\n");
        }

        private static void UpdateMessage()
        {
            Console.WriteLine("Updating a message from Storage Que");

            // Get the message from the queue and update the message contents.
            CloudQueueMessage message = queue.GetMessage();
            message.SetMessageContent("Updated contents.");
            queue.UpdateMessage(
                message,
                TimeSpan.FromSeconds(60.0),  // Make it invisible for another 60 seconds.
                MessageUpdateFields.Content | MessageUpdateFields.Visibility);

            Console.WriteLine($"Finished Updating Message Snippet\r\n");
        }

        private static void DeleteMessage()
        {
            /* Your code de-queues a message from a queue in two steps. When you call GetMessage, 
             * you get the next message in a queue. A message returned from GetMessage becomes 
             * invisible to any other code reading messages from this queue. By default, 
             * this message stays invisible for 30 seconds. To finish removing the message from the queue, 
             * you must also call DeleteMessage. This two-step process of removing a message assures that 
             * if your code fails to process a message due to hardware or software failure, another instance 
             * of your code can get the same message and try again. Your code calls DeleteMessage right 
             * after the message has been processed. */

            Console.WriteLine("Deleting a message from Storage Que");

            // Get the next message
            CloudQueueMessage retrievedMessage = queue.GetMessage();

            //Process the message in less than 30 seconds, and then delete the message
            queue.DeleteMessage(retrievedMessage);

            Console.WriteLine($"Finished Delete Message Snippet\r\n");
        }

        private static void CreateManyMessages()
        {
            Console.WriteLine("Creating 32 messages");

            for(var i = 0; i < 32; i++)
            {
                queue.AddMessage(new CloudQueueMessage($"Bulk add #{i}"));
            }

            Console.WriteLine($"Finished create many Messages Snippet\r\n");
        }

        private static void GetQueLength()
        {
            Console.WriteLine("Getting the que length");

            // Fetch the queue attributes.
            queue.FetchAttributes();

            // Retrieve the cached approximate message count.
            int? cachedMessageCount = queue.ApproximateMessageCount;

            // Display number of messages.
            // note: this is only an estimate
            Console.WriteLine("Number of messages in queue: " + cachedMessageCount);

            Console.WriteLine($"Finished Get Que Length Snippet\r\n");
        }

        private static void GetAndDeleteManyMessages()
        {
            Console.WriteLine("Getting a batch of 32 messages and deleting them");

            // you can get a batch of messages (up to 32)
            // note that it also sets each messages visibility timeout to 5 minutes while pulling them
            // Note that the 5 minutes starts for all messages at the same time, 
            // so after 5 minutes have passed since the call to GetMessages, 
            // any messages which have not been deleted will become visible again.
            foreach (CloudQueueMessage message in queue.GetMessages(32, TimeSpan.FromMinutes(5)))
            {
                Console.WriteLine($"Deleting {message.AsString}");
                // Process all messages in less than 5 minutes, deleting each message after processing.
                queue.DeleteMessage(message);
            }

            Console.WriteLine($"Finished get and delete many Messages Snippet\r\n");
        }

        private static void DeleteQue()
        {
            Console.WriteLine("Deleting the que");

            queue.Delete();

            Console.WriteLine($"Finished Delete Que Snippet\r\n");
        }
    }
}
