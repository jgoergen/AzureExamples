go through: https://blogs.msdn.microsoft.com/windowsazurestorage/2010/11/06/how-to-get-most-out-of-windows-azure-tables/
go through: https://blogs.msmvps.com/nunogodinho/2013/11/20/windows-azure-storage-performance-best-practices/
go through: https://blogs.msdn.microsoft.com/windowsazurestorage/2011/09/15/windows-azure-tables-introducing-upsert-and-query-projection/
go through: https://blogs.msdn.microsoft.com/windowsazurestorage/2014/09/08/cross-post-managing-concurrency-in-microsoft-azure-storage/
go through: https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-overview
go through: https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-design-guide
go through: https://www.codeproject.com/Tips/671375/Some-tips-and-tricks-for-azure-table-storage
   
## Azure Table storage limits


| Resource | Target |
| ------------- | -----:|
|Max size of single table|500 TiB|
|Max size of a table entity| MiB|
|Max number of properties in a table entity|255 (including 3 system properties: PartitionKey, RowKey and Timestamp)|
|Max number of stored access policies per table|5|
|Maximum request rate per storage account|20,000 transactions per second (assuming 1 KiB entity size)|
|Target throughput for single table partition (1 KiB entities)|Up to 2000 entities per second|

## Table Storage
------------------------------

Tables offer NoSQL storage for unstructured and semi-structured data—ideal for web applications, address books, and other user data.  

Data storage prices  

| STORAGE CAPACITY | LRS | GRS | RA-GRS | ZRS |
| ------------- | :-----: | :-----: | :-----: | -----: |
|First 1 terabyte (TB) / month|$0.07 per GB|$0.095 per GB|$0.12 per GB|$0.0875 per GB|
|Next 49 TB (1 to 50 TB) / month|$0.065 per GB|$0.08 per GB|$0.10 per GB|$0.0813 per GB|
|Next 450 TB (50 to 500 TB) / month|$0.06 per GB|$0.07 per GB|$0.09 per GB|$0.075 per GB|
|Next 500 TB (500 to 1,000 TB) / month|$0.055 per GB|$0.065 per GB|$0.08 per GB|$0.0688 per GB|
|Next 4,000 TB (1,000 to 5,000 TB) / month|$0.045 per GB|$0.06 per GB|$0.075 per GB|$0.0563 per GB|
|Over 5,000 TB / Month|Contact us|Contact us|Contact us|Contact us|

Operations and data transfer prices  

We charge $0.00036 per 10,000 transactions for tables. Any type of operation against the storage is counted as a transaction, including reads, writes, and deletes.  

## entity uniqueness

In Azure tables, entity uniqueness in a table is determined by the combination of PartitionKey (which physically groups related entities together for efficiency) and RowKey (a unique key per-partition)  

## Managing Concurrency in the File Service

The file service can be accessed using two different protocol endpoints – SMB and REST. The REST service does not have support for either optimistic locking or pessimistic locking and all updates will follow a last writer wins strategy. SMB clients that mount file shares can leverage file system locking mechanisms to manage access to shared files – including the ability to perform pessimistic locking. When an SMB client opens a file, it specifies both the file access and share mode. Setting a File Access option of "Write" or "Read/Write" along with a File Share mode of "None" will result in the file being locked by an SMB client until the file is closed. If REST operation is attempted on a file where an SMB client has the file locked the REST service will return status code 409 (Conflict) with error code SharingViolation.  

When an SMB client opens a file for delete, it marks the file as pending delete until all other SMB client open handles on that file are closed. While a file is marked as pending delete, any REST operation on that file will return status code 409 (Conflict) with error code SMBDeletePending. Status code 404 (Not Found) is not returned since it is possible for the SMB client to remove the pending deletion flag prior to closing the file. In other words, status code 404 (Not Found) is only expected when the file has been removed. Note that while a file is in a SMB pending delete state, it will not be included in the List Files results.Also note that the REST Delete File and REST Delete Directory operations are committed atomically and do not result in pending delete state.  

## Partition and Row keys

Specifying both partition and row keys in a query is the fastest way to retrieve a single entity from the Table service.  

## Batch operation speed

Serial batches should be faster than individual inserts due to sql server behavior. (Individual inserts are essentially little transactions that each flush to disk, while a real transaction flushes to disk as a group)."  

Depending on transaction isolation level, the batches will have limited ability to execute in parallel  

Batch size of 100 each batch with a unique PartitionKey, all in parallel, seems to offer the best real performance, turn off naggle, turn off expect 100, beef up the connection limit.  

Also make sure you are not accidentally inserting duplicates, that will cause an error and slow everything way way way down.  

### increasing the connection limit  
1) ervicePoint tableServicePoint = ServicePointManager.FindServicePoint(_StorageAccount.TableEndpoint);  
2) tableServicePoint.ConnectionLimit = 1000;  
3) increasing your threadpool: ThreadPool.SetMinThreads(1024, 256);  

## more batch operation notes
You can insert a batch of entities into a table in one write operation. Some other notes on batch operations:
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
By default a property is created as type String, unless you specify a different type
