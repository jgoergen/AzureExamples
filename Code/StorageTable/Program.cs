using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace StorageTable
{
    class Program
    {
        /* NOTE!!
        Developers already familiar with Azure Table storage may have used the WindowsAzure.Storage package in the past. It is recommended that 
        all new table applications use the Azure Storage Common Library and the Azure Cosmos DB Table Library, however the WindowsAzure.Storage 
        package is still supported. If you use the WindowsAzure.Storage library, include Microsoft.WindowsAzure.Storage.Table in your using statements.*/

        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=teststorage123jg;AccountKey=bdfO2NVHNcWGHok7jayK46gJtBUUkDPQljloMgIhn4FlgFWasR9igEQjumBqk78WI1rvUcf3jPYW6vo3GsShyw==;EndpointSuffix=core.windows.net";
        private const string tableName = "testtable2";

        private static CloudStorageAccount storageAccount;
        private static CloudTableClient tableClient;
        private static CloudTable table;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Azure Storage Table Examples");
            Console.WriteLine("Connecting to Storage Table");

            storageAccount = CloudStorageAccount.Parse(connectionString);
            tableClient = storageAccount.CreateCloudTableClient();

            // pull an extreme batch test out of here
            // https://stackoverflow.com/questions/17955557/painfully-slow-azure-table-insert-and-delete-batch-operations

            // run tests
            Console.WriteLine("Starting example snippets\r\n");
            CreateIfNotExists();
            AddEntityToTable();
            BatchAddEntries();
            GetAllEntriesInPartition();
            GetAllEntriesInPartitionByRowKey();
            ReplaceEntity();
            GetSingleEntryInPartitionByRowKey();
            InsertOrReplaceEntity();
            GetSubsetOfDataFromEntities();
            DeleteEntity();
            MixedBatchOperations();
            DeleteTable();

            // merge?
            // insertormerge?

            Console.WriteLine($"Examples completed, press enter to quit.");
            Console.ReadLine();
        }

        private static void CreateIfNotExists()
        {
            Console.WriteLine("Getting reference to Storage Table");

            table = tableClient.GetTableReference(tableName);

            Console.WriteLine("Creating Storage Table if it doesn't already exist");

            // Create the table if it doesn't already exist
            if (table.CreateIfNotExists())
            {
                Console.WriteLine("Table has been created.");
            }
            else
            {
                Console.WriteLine("Table already exists!");
            }

            Console.WriteLine($"Finished Create Table if not exist Snippet\r\n");
        }

        private static void AddEntityToTable()
        {
            Console.WriteLine("Adding new Entity to the Table");

            // create a new customer entity. 
            // the rowkey must be unique for this pratition!
            CustomerEntity customer1 = new CustomerEntity("Harp", "Walter");
            customer1.Email = "Walter@contoso.com";
            customer1.PhoneNumber = "425-555-0101";

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customer1);

            // Execute the insert operation.
            table.Execute(insertOperation);

            Console.WriteLine($"Finished Add Entity to Table Snippet\r\n");
        }

        private static void BatchAddEntries()
        {
            /*You can insert a batch of entities into a table in one write operation. Some other notes on batch operations:
                You can perform updates, deletes, and inserts in the same single batch operation.
                A single batch operation can include up to 100 entities.
                All entities in a single batch operation must have the same partition key.
                While it is possible to perform a query as a batch operation, it must be the only operation in the batch.
                
             Azure Table Storage batch operation is atomic so it is expected to return on the first failed operation. A batch operation may contain 
             1000 operations, there is not much point for the table service to keep executing all operations after it detected the first failure
             
             Tables don’t enforce a schema on entities, which means a single table can contain entities that have different sets of properties. An account can contain many tables, the size of which is only limited by the 100TB storage account limit.
             An entity is a set of properties, similar to a database row. An entity can be up to 1MB in size.
             A property is a name-value pair. Each entity can include up to 252 properties to store data. Each entity also has 3 system properties that specify a partition key, a row key, and a timestamp. Entities with the same partition key can be queried more quickly, and inserted/updated in atomic operations. An entity’s row key is its unique identifier within a partition.
             A property value may be up to 64 KB in size.
             By default a property is created as type String, unless you specify a different type*/

            Console.WriteLine("Adding 2 Entries at once with a Batch Operation");

            // Create a customer entity and add it to the table.
            CustomerEntity customer1 = new CustomerEntity("Smith", "Jeff");
            customer1.Email = "Jeff@contoso.com";
            customer1.PhoneNumber = "425-555-0104";

            // Create another customer entity and add it to the table.
            CustomerEntity customer2 = new CustomerEntity("Smith", "Ben");
            customer2.Email = "Ben@contoso.com";
            customer2.PhoneNumber = "425-555-0102";

            // Create the batch operation.
            TableBatchOperation batchOperation = new TableBatchOperation();

            // Add both customer entities to the batch insert operation.
            batchOperation.Insert(customer1);
            batchOperation.Insert(customer2);

            // Execute the batch operation.
            table.ExecuteBatch(batchOperation);

            Console.WriteLine($"Finished Batch Entries Snippet\r\n");
        }

        private static void MixedBatchOperations()
        {
            /*You can insert a batch of entities into a table in one write operation. Some other notes on batch operations:
                You can perform updates, deletes, and inserts in the same single batch operation.
                A single batch operation can include up to 100 entities.
                All entities in a single batch operation must have the same partition key.
                While it is possible to perform a query as a batch operation, it must be the only operation in the batch.*/
                
            // Create a customer entity and add it to the table.
            CustomerEntity customer1 = new CustomerEntity("Smith", "Frank");
            customer1.Email = "Frank@contoso.com";
            customer1.PhoneNumber = "425-425-3104";

            CustomerEntity customer2 = new CustomerEntity("Smith", "Terry");
            customer2.Email = "Charles@contoso.com";
            customer2.PhoneNumber = "623-535-3102";
            
            // Create the batch operation.
            TableBatchOperation batchOperation = new TableBatchOperation();

            Console.WriteLine("Getting all entities with the partition key of 'Smith' for deletion");

            // Create the table query.
            TableQuery<CustomerEntity> rangeQuery = new TableQuery<CustomerEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"));

            // Loop through the results, displaying information about the entity.
            foreach (CustomerEntity entity in table.ExecuteQuery(rangeQuery))
            {
                Console.WriteLine($"Deleting {entity.PartitionKey} {entity.RowKey}");
                batchOperation.Delete(entity);
            }

            // Add both customer entities to the batch insert operation.
            Console.WriteLine($"Inserting {customer1.PartitionKey} {customer1.RowKey}");
            batchOperation.Insert(customer1);

            Console.WriteLine($"Inserting/Replacing {customer2.PartitionKey} {customer2.RowKey}");
            batchOperation.InsertOrReplace(customer2);

            // Execute the batch operation.
            table.ExecuteBatch(batchOperation);

            Console.WriteLine($"Finished Mixed Batch Operations Snippet\r\n");
        }
        
        private static void GetAllEntriesInPartition()
        {
            Console.WriteLine("Pulling back all entries from partition where the parition key is 'Smith'");

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<CustomerEntity> query = 
                new TableQuery<CustomerEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"));

            // Print the fields for each customer.
            foreach (CustomerEntity entity in table.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                    entity.Email, entity.PhoneNumber);
            }

            Console.WriteLine($"Finished Get All Entries in Partition Snippet\r\n");
        }

        private static void GetAllEntriesInPartitionByRowKey()
        {
            Console.WriteLine("Pulling back all entries from partition where the parition key is 'Smith' and a rowkey that starts with A-E");

            // Create the table query.
            TableQuery<CustomerEntity> rangeQuery = new TableQuery<CustomerEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, "E")));

            // Loop through the results, displaying information about the entity.
            foreach (CustomerEntity entity in table.ExecuteQuery(rangeQuery))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                    entity.Email, entity.PhoneNumber);
            }

            Console.WriteLine($"Finished Get All Entries in Partition by Rowkey Snippet\r\n");
        }

        private static void GetSingleEntryInPartitionByRowKey()
        {
            Console.WriteLine("Pulling back an entry from partition where the parition key is 'Smith' and a rowkey is 'Ben'");

            // Specifying both partition and row keys in a query is the fastest way to retrieve a single entity from the Table service.

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Smith", "Ben");

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Print the phone number of the result.
            if (retrievedResult.Result != null)
            {
                Console.WriteLine(((CustomerEntity)retrievedResult.Result).PhoneNumber);
            }
            else
            {
                Console.WriteLine("The phone number could not be retrieved.");
            }

            Console.WriteLine($"Finished Get Single Entry in Partition by Rowkey Snippet\r\n");
        }

        private static void ReplaceEntity()
        {
            /*Replace causes the entity to be fully replaced on the server, unless the entity on the server has changed since it was retrieved, 
             * in which case the operation will fail. This failure is to prevent your application from inadvertently overwriting a change made between 
             * the retrieval and update by another component of your application. The proper handling of this failure is to retrieve the entity again, 
             * make your changes (if still valid), and then perform another Replace operation. The next section will show you how to override this behavior.*/

            Console.WriteLine("Getting an entity to alter.");

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Smith", "Ben");

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a CustomerEntity object.
            CustomerEntity updateEntity = (CustomerEntity)retrievedResult.Result;

            Console.WriteLine("Updating entity");

            if (updateEntity != null)
            {
                // Change the phone number.
                updateEntity.PhoneNumber = "555-555-5555";

                Console.WriteLine("Replacing entity");

                // Create the Replace TableOperation.
                TableOperation updateOperation = TableOperation.Replace(updateEntity);

                // Execute the operation.
                table.Execute(updateOperation);

                Console.WriteLine("Entity updated.");
            }
            else
            {
                Console.WriteLine("Entity could not be retrieved.");
            }

            Console.WriteLine($"Finished Replace Entity Snippet\r\n");
        }

        private static void InsertOrReplaceEntity()
        {
            /*Replace operations will fail if the entity has been changed since it was retrieved from the server. Furthermore, you must retrieve the 
             * entity from the server first in order for the Replace operation to be successful. Sometimes, however, you don't know if the entity 
             * exists on the server and the current values stored in it are irrelevant. Your update should overwrite them all. To accomplish this, 
             * you would use an InsertOrReplace operation. This operation inserts the entity if it doesn't exist, or replaces it if it does, regardless 
             * of when the last update was made.*/

            Console.WriteLine("Creating a new entity");

            // Create a customer entity.
            CustomerEntity customer3 = new CustomerEntity("Jones", "Fred");
            customer3.Email = "Fred@contoso.com";
            customer3.PhoneNumber = "425-555-0106";

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customer3);

            Console.WriteLine("Inserting entity");

            // Execute the operation.
            table.Execute(insertOperation);

            Console.WriteLine("Creating a new entity with the same partition / row keys to replace the other one");

            // Create another customer entity with the same partition key and row key.
            // We've already created a 'Fred Jones' entity and saved it to the
            // 'people' table, but here we're specifying a different value for the
            // PhoneNumber property.
            CustomerEntity customer4 = new CustomerEntity("Jones", "Fred");
            customer4.Email = "Fred@contoso.com";
            customer4.PhoneNumber = "425-555-0107";

            // Create the InsertOrReplace TableOperation.
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(customer4);

            Console.WriteLine("Replacing entity");

            // Execute the operation. Because a 'Fred Jones' entity already exists in the
            // 'people' table, its property values will be overwritten by those in this
            // CustomerEntity. If 'Fred Jones' didn't already exist, the entity would be
            // added to the table.
            table.Execute(insertOrReplaceOperation);

            Console.WriteLine($"Finished Insert or Replace Entity Snippet\r\n");
        }

        private static void GetSubsetOfDataFromEntities()
        {
            /*A table query can retrieve just a few properties from an entity instead of all the entity properties. This technique, called projection, 
             * reduces bandwidth and can improve query performance, especially for large entities. The query in the following code returns only the email 
             * addresses of entities in the table. */

            Console.WriteLine("Pulling back only the email field from all entities in our table");

            // Define the query, and select only the Email property.
            TableQuery<DynamicTableEntity> projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "Email" });

            // Define an entity resolver to work with the entity after retrieval.
            EntityResolver<string> resolver = (pk, rk, ts, props, etag) => props.ContainsKey("Email") ? props["Email"].StringValue : null;

            foreach (string projectedEmail in table.ExecuteQuery(projectionQuery, resolver, null, null))
            {
                Console.WriteLine(projectedEmail);
            }

            Console.WriteLine($"Finished Get Subset of Data from Entities Snippet\r\n");
        }

        private static void DeleteEntity()
        {
            Console.WriteLine("Getting an entity to delete");

            // Create a retrieve operation that expects a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Smith", "Ben");

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a CustomerEntity.
            CustomerEntity deleteEntity = (CustomerEntity)retrievedResult.Result;

            // Create the Delete TableOperation.
            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                Console.WriteLine("Deleting it");

                // Execute the operation.
                table.Execute(deleteOperation);
            }
            else
            {
                Console.WriteLine("Could not retrieve the entity.");
            }

            Console.WriteLine($"Finished Delete Entity Snippet\r\n");
        }

        private static void DeleteTable()
        {
            Console.WriteLine("Deleting the table");

            // Delete the table it if exists.
            table.DeleteIfExists();

            Console.WriteLine($"Finished Delete Table Snippet\r\n");
        }


    }

    /* Entities map to C# objects by using a custom class derived from TableEntity. To add an entity to a table, create a class that defines the 
     * properties of your entity. The following code defines an entity class that uses the customer's first name as the row key and last name as 
     * the partition key. Together, an entity's partition and row key uniquely identify it in the table. Entities with the same partition key can 
     * be queried faster than entities with different partition keys, but using diverse partition keys allows for greater scalability of parallel 
     * operations. Entities to be stored in tables must be of a supported type, for example derived from the TableEntity class. Entity properties 
     * you'd like to store in a table must be public properties of the type, and support both getting and setting of values. Also, your entity type 
     * must expose a parameter-less constructor.*/

    public class CustomerEntity : TableEntity
    {
        public CustomerEntity(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }

        // note! it's required that you have a constructor that takes no parameters!
        public CustomerEntity() { }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
