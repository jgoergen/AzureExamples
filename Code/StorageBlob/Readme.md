go through: https://docs.microsoft.com/en-us/azure/storage/common/storage-dotnet-shared-access-signature-part-1

Creating storage accounts via azure cli  
https://docs.microsoft.com/en-us/cli/azure/storage/account?view=azure-cli-latest#az-storage-account-create

# Replication Options
Premium_LRS, Standard_GRS, Standard_LRS, Standard_RAGRS, Standard_ZRS 

## LRS (Locally redundant storage)
   3 copies withing a region of the storage account  
   
   Locally redundant storage (LRS) is designed to provide at least 99.999999999% (11 9's) durability of objects over a given year by replicating your data within a storage scale unit. A storage scale unit is hosted in a datacenter in the region in which you created your storage account. A write request to an LRS storage account returns successfully only after the data has been written to all replicas. These replicas each reside in separate fault domains and update domains within one storage scale unit.  
    
   A storage scale unit is a collection of racks of storage nodes. A fault domain (FD) is a group of nodes that represent a physical unit of failure and can be considered as nodes belonging to the same physical rack. An upgrade domain (UD) is a group of nodes that are upgraded together during the process of a service upgrade (rollout). The replicas are spread across UDs and FDs within one storage scale unit. This architecture ensures that your data is available if a hardware failure impacts a single rack or when nodes are upgraded during a rollout.
    
   LRS is the lowest cost replication option and offers the least durability compared to other options. If a datacenter-level disaster (for example, fire or flooding) occurs, all replicas may be lost or unrecoverable. To mitigate this risk, Microsoft recommends using either zone-redundant storage (ZRS) or geo-redundant storage (GRS).

## ZRS (Zone redundant storage)
   3 copies withing a region of the storage account, but each copy is on it's own 'rack' or availability zone
   
   Zone-redundant storage (ZRS) replicates your data synchronously across three storage clusters in a single region. Each storage cluster is physically separated from the others and resides in its own availability zone (AZ). Each availability zone, and the ZRS cluster within it, is autonomous, with separate utilities and networking capabilities.
   
   Storing your data in a ZRS account ensures that you will be able access and manage your data in the event that a zone becomes unavailable. ZRS provides excellent performance and low latency. ZRS offers the same scalability targets as locally-redundant storage (LRS).
   
   Consider ZRS for scenarios that require strong consistency, strong durability, and high availability even if an outage or natural disaster renders a zonal data center unavailable. ZRS offers durability for storage objects of at least 99.9999999999% (12 9's) over a given year.

## GRS means Globally redundant storage
   3 copies in a primary and 3 in another data center, but you can only read from the second data center if the first goes down
   
   Geo-redundant storage (GRS) is designed to provide at least 99.99999999999999% (16 9's) durability of objects over a given year by replicating your data to a secondary region that is hundreds of miles away from the primary region. If your storage account has GRS enabled, then your data is durable even in the case of a complete regional outage or a disaster in which the primary region is not recoverable.
   
   ### If you opt for GRS, you have two related options to choose from:  
  1) GRS replicates your data to another data center in a secondary region, but that data is available to be read only if Microsoft initiates a failover from the primary to secondary region.  
  2) Read-access geo-redundant storage (RA-GRS) is based on GRS. RA-GRS replicates your data to another data center in a secondary region, and also provides you with the option to read from the secondary region. With RA-GRS, you can read from the secondary regardless of whether Microsoft initiates a failover from the primary to the secondary.  
   
   For a storage account with GRS or RA-GRS enabled, all data is first replicated with locally-redundant storage (LRS). An update is first committed to the primary location and replicated using LRS. The update is then replicated asynchronously to the secondary region using GRS. When data is written to the secondary location, it is also replicated within that location using LRS.
   
   Both the primary and secondary regions manage replicas across separate fault domains and upgrade domains within a storage scale unit. The storage scale unit is the basic replication unit within the datacenter. Replication at this level is provided by LRS; 
   
   When you create a storage account, you select the primary region for the account. The paired secondary region is determined based on the primary region, and cannot be changed

## RAGRS (Read Access Globally Redundant Storage)
   3 copies in a primary region and 3 in another, with read access to both all the time.
   
   Read-access geo-redundant storage (RA-GRS) maximizes availability for your storage account. RA-GRS provides read-only access to the data in the secondary location, in addition to geo-replication across two regions.
   
   When you enable read-only access to your data in the secondary region, your data is available on a secondary endpoint as well as on the primary endpoint for your storage account. The secondary endpoint is similar to the primary endpoint, but appends the suffix –secondary to the account name. For example, if your primary endpoint for the Blob service is myaccount.blob.core.windows.net, then your secondary endpoint is myaccount-secondary.blob.core.windows.net. The access keys for your storage account are the same for both the primary and secondary endpoints.
   
### Some considerations to keep in mind when using RA-GRS:  
1) Your application has to manage which endpoint it is interacting with when using RA-GRS.  
2) Since asynchronous replication involves a delay, changes that have not yet been replicated to the secondary region may be lost if data cannot be recovered from the primary region, for example in the event of a regional disaster.  
3) You can check the Last Sync Time of your storage account. Last Sync Time is a GMT date/time value. All primary writes before the Last Sync Time have been successfully written to the secondary location, meaning that they are available to be read from the secondary location. Primary writes after the Last Sync Time may or may not be available for reads yet. You can query this value using the Azure portal, Azure PowerShell, or from one of the Azure Storage client libraries.  
4) If Microsoft initiates failover to the secondary region, you will have read and write access to that data after the failover has completed. For more information, see Disaster Recovery Guidance.  
5) For information on how to switch to the secondary region, see What to do if an Azure Storage outage occurs.  
6) RA-GRS is intended for high-availability purposes. For scalability guidance, review the performance checklist.  
8) For suggestions on how to design for high availability with RA-GRS, see Designing Highly Available Applications using RA-GRS storage.  
 
   Prices for locally redundant storage (LRS) Archive Block Blob start from: $0.002/GB per month
   
   Prices for LRS File storage start from: $0.06/GB per month  
   
   When you create a storage account, you select the primary region for the account. The paired secondary region is determined based on the primary region, and cannot be changed

## Storage account regional location  
In any distributed environment, placing the client near to the server delivers in the best performance. For accessing Azure Storage with the lowest latency, the best location for your client is within the same Azure region. For example, if you have an Azure Web Site that uses Azure Storage, you should locate them both within a single region (for example, US West or Asia Southeast). This reduces the latency and the cost — at the time of writing, bandwidth usage within a single region is free.  

If your client applications are not hosted within Azure (such as mobile device apps or on premises enterprise services), then again placing the storage account in a region near to the devices that will access it, will generally reduce latency. If your clients are broadly distributed (for example, some in North America, and some in Europe), then you should consider using multiple storage accounts: one located in a North American region and one in a European region. This will help to reduce latency for users in both regions. This approach is usually easier to implement if the data the application stores is specific to individual users, and does not require replicating data between storage accounts. For broad content distribution, a CDN is recommended – see the next section for more details.  

## Data security
You can encrypt data at rest as an option when you create the storage account  

You can also connect via https, but the only way to do this with a custom domain is via a CDN  

## Auto Retries when accessing blobs
By default, only some failures are retried. For example, connection failures and throttling failures can be retried. Resource not found (404) or authentication 
failures are not retried, because these are not likely to succeed on retry.  
If not set, the Storage Client uses an exponential backoff retry policy, where the wait time gets exponentially longer between requests, up to a total of around 30 seconds.  
The default retry policy is recommended for most scenarios.  

### Container access types
1) private / off: no one can get the the data within over the public internet
2) blob: public access for downloading only, but only with the url to the files over the internet
3) container: public access to container file list and files over the internet
    
When you need to authorize code such as JavaScript in a user's web browser or a mobile phone app to access data in Azure Storage, one approach is to use an application in web role as a proxy: the user's device authenticates with the web role, which in turn authorizes access to storage resources. In this way, you can avoid exposing your storage account keys on insecure devices. However, this places a big overhead on the web role because all the data transferred between the user's device and the storage service must pass through the web role. You can avoid using a web role as a proxy for the storage service by using Shared Access Signatures (SAS), sometimes in conjunction with Cross-Origin Resource Sharing headers (CORS). Using SAS, you can allow your user's device to make requests directly to a storage service by means of a limited access token. For example, if a user wants to upload a photo to your application, your web role can generate and send to the user's device a SAS token that grants permission to write to a specific blob or container for the next 30 minutes (after which the SAS token expires).
   
## Storage Account access limitations

At the time of writing, the bandwidth targets in the US for a geo-redundant storage (GRS) account are 10 gigabits per second (Gbps) for ingress (data sent to the storage account) and 20 Gbps for egress (data sent from the storage account). For a locally redundant storage (LRS) account, the limits are higher – 20 Gbps for ingress and 30 Gbps for egress. International bandwidth limits may be lower and can be found on our scalability targets page. 

When a Storage service throttles your application, the service begins to return "503 Server busy" or "500 Operation timeout" error codes for some storage transactions. 

## Blob type
1) Page blobs: Random access, always used for VM Images  
2) Block blob: Read access ( upload once, occasionally replace maybe, read many times. )  
3) Append blob: Write access to the end ( mostly for logs. write many times to the end, read occassionally )  

## Blob type storage specific access tiers
1) Cool: Less frequently accessed, like backups. Costs less  
2) Hot: Frequently accessed, costs more  

## Data redundancy options
1) locally redundant storage:  Designed to provide at least 99.999999999 % (11 9's) durability of objects over a given year by keeping multiple copies of your data in one datacenter.  
2) zone redundant storage: Designed to provide at least 99.9999999999 % (12 9's) durability of objects over a given year by keeping multiple copies of your data across multiple datacenters or across regions.  
3) geographically redundant storage: Designed to provide at least 99.99999999999999 % (16 9's) durability of objects over a given year by keeping multiple copies of the data in one region, and asynchronously replicating to a second region.  
4) read-access geographically redundant storage: Designed for to provide at least 99.99999999999999 % (16 9's) durability of objects over a given year and 99.99 % read availability by allowing read access from the second region used for GRS.  

## Azure offers three types of storage accounts: 
1) General Purpose v2  
2) General Purpose v1  
3) Blob Storage   

Storage accounts determine eligibility for certain storage services and features, and each is priced differently. We recommend thoroughly reviewing the pricing models to determine which is the most appropriate, as some workloads can be priced very differently depending on which account type is used.  

For new customers, we generally recommend General Purpose v2 for access to the latest storage features, including Tiered Storage and Archive. We recommend General Purpose v1 and Blob Storage accounts only for customers with prior experience with those accounts, but these customers may also benefit from using GPv2. Learn more about the benefits of different storage account types.  

## Storage limits
For additional details on storage account limits, see Azure Storage Scalability and Performance Targets.

| Resource        | Default Limit  |
| ------------- | -----:|
|Number of storage accounts per region per subscription|2001|
|Max storage account capacity|500 TiB2|
|Max number of blob containers, blobs, file shares, tables, queues, entities, or messages per storage account|No limit|
|Maximum request rate per storage account|20,000 requests per second2|
|Max ingress3 per storage account (US Regions)|10 Gbps if RA-GRS/GRS enabled, 20 Gbps for LRS/ZRS4|
|Max egress3 per storage account (US Regions)|20 Gbps if RA-GRS/GRS enabled, 30 Gbps for LRS/ZRS4|
|Max ingress3 per storage account (Non-US regions)|5 Gbps if RA-GRS/GRS enabled, 10 Gbps for LRS/ZRS4|
|Max egress3 per storage account (Non-US regions)|10 Gbps if RA-GRS/GRS enabled, 15 Gbps for LRS/ZRS4|

If you require more than 200 storage accounts in a given region, make a request through Azure Support. The Azure Storage team will review your business case and may approve up to 250 storage accounts for a given region.  

If you need expanded limits for your storage account, please contact Azure Support. The Azure Storage team will review the request and may approve higher limits on a case by case basis. Both general-purpose and Blob storage accounts support increased capacity, ingress/egress, and request rate by request. For the new maximums for Blob storage accounts, see Announcing larger, higher scale storage accounts.  

Capped only by the account's ingress/egress limits. Ingress refers to all data (requests) being sent to a storage account. Egress refers to all data (responses) being received from a storage account.  

## Azure Blob storage limits
| Resource        | Target  |
| ------------- | -----:|
|Max size of single blob container|Same as max storage account capacity|
|Max number of blocks in a block blob or append blob|50,000 blocks|
|Max size of a block in a block blob|100 MiB|
|Max size of a block blob|50,000 X 100 MiB (approx. 4.75 TiB)|
|Max size of a block in an append blob|4 MiB|
|Max size of an append blob|50,000 x 4 MiB (approx. 195 GiB)|
|Max size of a page blob|8 TiB|
|Max number of stored access policies per blob container|5|
|Target throughput for single blob|Up to 60 MiB per second, or up to 500 requests per second|

# PAGE BLOBS

## Premium Page Blobs prices
Premium Storage Supports Page Blobs Only    

Premium Page Blobs are high-performance solid-state drive (SSD)-based storage, designed to support I/O-intensive workloads with significantly high throughput and low latency. Premium Page Blobs provide provisioned disk performance up to 7,500 IOPS and 250MBps per blob.  

You can attach Premium Page Blobs to VM sizes with the “s” denomination for Premium Unmanaged Disk support—for example, Dsv1, Dsv2, and Gs. It is also recommended to use Managed Disks for VM persistent Storage for higher availability.  

Pricing for Premium Page Blobs depends on the size of disks provisioned and Outbound data transfer.  

| PAGE BLOB TYPES | P10 | P20 | P30 | P40 | P50 | P60| 
| ------------- |:-------------:|:-------------:|:-------------:|:-------------:|:-------------:|-----:|
|Size|128 GB|512 GB|1 TB|2 TB|4 TB|8 TB|
|Price per month|$17.92|$66.56|$122.88|$235.52|$450.56|$860.16|
|IOPS per Page Blob|500|2,300|5,000|7,500|7,500|7,500|
|Throughput per Page Blob|100 MB/second|150 MB/second|200 MB/second|250 MB/second|250 MB/second|250 MB/second|

here are no transaction costs for Premium Page Blobs.  

Your Premium Page Blob is charged based on the lowest type that supports your target size.  

* Azure does not support attaching P60 as a disk to VMs at this time. P60 are only supported as Page Blobs accessible through REST.
Note: Snapshots are charged at $0.12/GB per month.  

## Standard Page Blob prices
Standard Page Blobs use hard disk drive (HDD)-based storage media. They are best suited for development, testing, and other infrequently accessed workloads that are less sensitive to performance variability.

### Prices for Standard Page Blobs depend on:
1) Volume of data stored per month.
2) Quantity and types of operations performed, plus outbound data transfer cost.
3) Data redundancy.

### Data storage prices for Standard Page Blobs
| LRS | ZRS | GRS | RA-GRS |
| ------------- |:-------------:|:-------------:| -----:|
|$0.045 per GB|$0.0563 per GB|$0.06 per GB|$0.075 per GB|

### Operations prices for Page Blobs used as Unmanaged Disks  
We charge $0.0005 per 10,000 transactions for Standard Page Blobs attached to VM and used as Unmanaged Disks. Any type of operation against against Unmanaged Disks is counted as a transaction including reads, writes, and deletes.  

### Operations prices for Page Blobs (Non-disks)
Page Blob transactions that are not performed through a VM are billed on a per-transaction basis. For each transaction, a single operation is billed as either a write, a read, or a delete operation. If a read or write transaction is larger than 64 KB, an additional read or write input/output (IO) is billed for each additional 64 KB, up to a maximum of 15 IOs per transaction. If the transaction is larger than 1 MB [64 KB x (1 transaction + 15 IOs)], no further IOs will be charged, regardless of the size of the transaction.

| | LRS | ZRS | GRS | RA-GRS |
| ------------- |:-------------:|:-------------:|:-------------:| -----:|
|Write operations* (per 10,000)|$0.015|$0.01875|$0.03|$0.03|
|Write additional IO units (per 10,000)|$0.0024|$0.003|$0.0047|$0.0047|
|Read operations** (per 10,000)|$0.0015|$0.0015|$0.0015|$0.0015|
|Read additional IO units (per 10,000)|$0.0002|$0.0002|$0.0002|$0.0002|
|Delete operations|Free|Free|Free|Free|

* The following API calls are considered write operations: AbortCopyBlob, AppendBlock, BreakBlobLease, BreakContainerLease, ChangeBlobLease, ChangeContainerLease, CopyBlob, CreateContainer, IncrementalCopyBlob, PutBlob, PutGeoMessage, PutGeoRepairMessageMeasurementEvent, PutGeoVerificationMessageMeasurementEvent, PutPage, SetBlobMetadata, SetBlobProperties, SetBlobServiceProperties, SetContainerACL, SetContainerMetadata, SnapshotBlob, and UndeleteBlob.  
** The following API calls are considered read operations: AcquireBlobLease, AcquireContainerLease, BlobPreflightRequest, GeoBootstrap, GetBlob, GetBlobLeaseInfo, GetBlobMetadata, GetBlobProperties, GetBlobServiceProperties, GetBlobServiceStats, GetBlockList, GetContainerACL, GetContainerMetadata, GetContainerProperties, GetCopyInformation, GetEncryptionKey, GetPageRegions, ReleaseBlobLease, ReleaseContainerLease, RenewBlobLease, and RenewContainerLease.  

## Container and blob naming rules
You should be aware of several naming rules for blobs and their containers. A blob container name must be between 3 and 63 characters in length; start with a letter or number; and contain only letters, numbers, and the hyphen. All letters used in blob container names must be lowercase. Lowercase is required because using mixed-case letters in container names may be problematic. Locating trouble in a failing application related to the incorrect use of mixed-case letters might result in a lot of wasted time and endless amounts of frustration and confusion.  

To make matters a bit confusing, blob names can use mixed-case letters. In fact, a blob name can contain any combination of characters as long as the reserved URL characters are properly escaped. The length of a blob name can range from as short as 1 character to as long as 1024 characters.  

If you inadvertently violate any of these naming rules, you receive an HTTP 400 (Bad Request) error code from the data storage service, resulting in a StorageClientException being thrown if you are accessing blob storage using the Windows Azure software development kit (SDK).  

You are not prohibited from using mixed casing in code, though, but some irregularities may adversely impact you when you do use it. For example, if you create a container properly in lowercase, but then later attempt to use that container in mixed-cased requests, your requests will all succeed because the mixed case container name is silently matched with the lowercase container name. This silent but menacing casing coercion can lead you to really scratch your head during debugging, so I strongly urge you to commit to memory the rule that blob container names must not contain uppercase letters.  

Avoid blob names that end with a dot (.), a forward slash (/), or a sequence or combination of the two.  

## Directories
The Blob service is based on a flat storage scheme, not a hierarchical scheme. However, you may specify a character or string delimiter within a blob name to create a virtual hierarchy. For example, the following list shows valid and unique blob names. Notice that a string can be valid as both a blob name and as a virtual directory name in the same container:  
    /a  
    /a.txt  
    /a/b  
    /a/b.txt  

You can take advantage of the delimiter character when enumerating blobs.  

## Notes on Container Meta Data!
Metadata name/value pairs may contain only ASCII characters. Metadata name/value pairs are valid HTTP headers, and so must adhere to all restrictions governing HTTP headers. It's recommended that you use URL encoding or Base64 encoding for names and values containing non-ASCII characters.  

The name of your metadata must conform to the naming conventions for C# identifiers.  

## Resource URI Syntax
Each resource has a corresponding base URI, which refers to the resource itself.  

For the storage account, the base URI includes the name of the account only: https://myaccount.blob.core.windows.net  

For a container, the base URI includes the name of the account and the name of the container:  https://myaccount.blob.core.windows.net/mycontainer  

For a blob, the base URI includes the name of the account, the name of the container, and the name of the blob: https://myaccount.blob.core.windows.net/mycontainer/myblob  

A storage account may have a root container, a default container that can be omitted from the URI. A blob in the root container can be referenced without naming the container, or the root container can be explicitly referenced by its name ($root). See Working with the Root Container for more information. The following URIs both refer to a blob in the root container: https://myaccount.blob.core.windows.net/myblob and https://myaccount.blob.core.windows.net/$root/myblob  

## Blob Snapshots
A snapshot is a read-only version of a blob stored as it was at the time the snapshot was created. You can use snapshots to create a backup or checkpoint of a blob. A snapshot blob name includes the base blob URI plus a date-time value that indicates when the snapshot was created.

For example, assume that a blob has the following URI: https://myaccount.blob.core.windows.net/mycontainer/myblob  

The URI for a snapshot of that blob is formed as follows: https://myaccount.blob.core.windows.net/mycontainer/myblob?snapshot=<DateTime> 

## Timeouts
A call to a Blob service API can include a server timeout interval, specified in the timeout parameter of the request URI. If the server timeout interval elapses before the service has finished processing the request, the service returns an error.  

The maximum timeout interval for Blob service operations is 30 seconds, with some exceptions noted below. Apart from these exceptions, the Blob service automatically reduces any timeouts larger than 30 seconds to the 30-second maximum.  

The following example REST URI sets the timeout interval for the List Containers operation to 20 seconds: GET https://myaccount.blob.core.windows.net?comp=list&timeout=20   

## Exceptions to Default Timeout Interval
The following operations implement exceptions to the standard 30 second timeout interval:  

Calls to get a blob, get page ranges, or get a block list are permitted 2 minutes per megabyte to complete. If an operation is taking longer than 2 minutes per megabyte on average, it will time out.  

Calls to write a blob, write a block, or write a page are permitted 10 minutes per megabyte to complete. If an operation is taking longer than 10 minutes per megabyte on average, it will time out.  

The maximum timeout to write a block list is 60 seconds.  

A container that was recently deleted cannot be recreated until all of its blobs are deleted. Depending on how much data was stored within the container, complete deletion can take seconds or minutes. If you try to create a container of the same name during this cleanup period, your call returns an error immediately.  

## ETAGS
An ETag property is used for optimistic concurrency during updates. It is not a timestamp as there is another property called 
TimeStamp that stores the last time a record was updated. For example, if you load an entity and want to update it, the ETag 
must match what is currently stored. This is important b/c if you have  

Every change to a record updates the ETag stored for that record. So when someone tries to save a file with an ETag that is conflicting, then it won’t match and you can handle what should be done.  

If you don’t care that the changes and want to overwrite them, then you can pass “*” with the save and Azure won’t give an error when the ETag does not match.  

string ETag (r/o) – this is an identifier for a specific version of a resource. This is used for web cache validation, and allows a client to make conditional requests. ETags are also used for optimistic concurrently control. For example, if you were reading a blob and saved the ETag, then did some other processing and came back to upload a new version of that blob, you could read the ETag again and check it against the prior value. If the ETag values match, then the file hasn’t changed and you can upload a new version of it. If the match fails, the storage service will return a 412 error (precondition failed).  

The Storage service assigns an identifier to every object stored. This identifier is updated every time an update operation is performed on an object. The identifier is returned to the client as part of an HTTP GET response using the ETag (entity tag) header that is defined within the HTTP protocol. A user performing an update on such an object can send in the original ETag along with a conditional header to ensure that an update will only occur if a certain condition has been met – in this case the condition is an “If-Match” header which requires the Storage Service to ensure the value of the ETag specified in the update request is the same as that stored in the Storage Service.  

### The outline of this process is as follows:
1) Retrieve a blob from the storage service, the response includes an HTTP ETag Header value that identifies the current version of the object in the storage service.
2) When you update the blob, include the ETag value you received in step 1 in the If-Match conditional header of the request you send to the service.
3) The service compares the ETag value in the request with the current ETag value of the blob.
4) If the current ETag value of the blob is a different version than the ETag in the If-Match conditional header in the request, the service returns a 412 error to the client. This indicates to the client that another process has updated the blob since the client retrieved it.
5) If the current ETag value of the blob is the same version as the ETag in the If-Match conditional header in the request, the service performs the requested operation and updates the current ETag value of the blob to show that it has created a new version.

## Pessimistic concurrency for blobs
To lock a blob for exclusive use, you can acquire a lease on it. When you acquire a lease, you specify for how long you need the lease: this can be for between 15 to 60 seconds or infinite which amounts to an exclusive lock. You can renew a finite lease to extend it, and you can release any lease when you are finished with it. The blob service automatically releases finite leases when they expire.  

Leases enable different synchronization strategies to be supported, including exclusive write / shared read, exclusive write / exclusive read and shared write / exclusive read. Where a lease exists the storage service enforces exclusive writes (put, set and delete operations) however ensuring exclusivity for read operations requires the developer to ensure that all client applications use a lease ID and that only one client at a time has a valid lease ID. Read operations that do not include a lease ID result in shared reads.  

If you attempt a write operation on a leased blob without passing the lease id, the request fails with a 412 error. Note that if the lease expires before calling the UploadText method but you still pass the lease id, the request also fails with a 412 error.   

Leases on containers enable the same synchronization strategies to be supported as on blobs (exclusive write / shared read, exclusive write / exclusive read and shared write / exclusive read) however unlike blobs the storage service only enforces exclusivity on delete operations. To delete a container with an active lease, a client must include the active lease ID with the delete request. All other container operations succeed on a leased container without including the lease ID in which case they are shared operations. If exclusivity of update (put or set) or read operations is required then developers should ensure all clients use a lease ID and that only one client at a time has a valid lease ID.  

