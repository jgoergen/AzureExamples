Creating storage accounts via azure cli
https://docs.microsoft.com/en-us/cli/azure/storage/account?view=azure-cli-latest#az-storage-account-create

create sku options
    Premium_LRS, Standard_GRS, Standard_LRS, Standard_RAGRS, Standard_ZRS
    LRS means Locally redundant storage
    GRS means Globally redundant storage
    RAGRS means read access globally redundant storage

Prices for locally redundant storage (LRS) Archive Block Blob start from: $0.002/GB per month
Prices for LRS File storage start from: $0.06/GB per month

Data redundancy options
    locally redundant storage	
        Designed to provide at least 99.999999999 % (11 9's) durability of objects over a given year by keeping multiple copies of your data in one datacenter.

    zone redundant storage	
        Designed to provide at least 99.9999999999 % (12 9's) durability of objects over a given year by keeping multiple copies of your data across multiple datacenters or across regions.

    geographically redundant storage	
        Designed to provide at least 99.99999999999999 % (16 9's) durability of objects over a given year by keeping multiple copies of the data in one region, and asynchronously replicating to a second region.

    read-access geographically redundant storage	
        Designed for to provide at least 99.99999999999999 % (16 9's) durability of objects over a given year and 99.99 % read availability by allowing read access from the second region used for GRS.

Azure now offers three types of storage accounts: 
    General Purpose v2, 
    General Purpose v1, 
    Blob Storage. 

Storage accounts determine eligibility for certain storage services and features, and each is priced differently. We recommend thoroughly reviewing the pricing models to determine which is the most appropriate, as some workloads can be priced very differently depending on which account type is used.
For new customers, we generally recommend General Purpose v2 for access to the latest storage features, including Tiered Storage and Archive. We recommend General Purpose v1 and Blob Storage accounts only for customers with prior experience with those accounts, but these customers may also benefit from using GPv2. Learn more about the benefits of different storage account types.

Todo: https://docs.microsoft.com/en-us/azure/storage/common/storage-performance-checklist#subheading39

## Azure Queue storage limits
Resource	Target
Max size of single queue	500 TiB
Max size of a message in a queue	64 KiB
Max number of stored access policies per queue	5
Maximum request rate per storage account	20,000 messages per second assuming 1 KiB message size
Target throughput for single queue (1 KiB messages)	Up to 2000 messages per second

Storage Que 
------------------------------

Queues provide a reliable messaging solution for your apps, and are generally used to store messages to be processed asynchronously. Queue messages can be up to 64 KB in size and can contain millions of messages.

Data storage prices

LRS	ZRS	GRS	RA-GRS
$0.045 per GB	$0.0563 per GB	$0.06 per GB	$0.075 per GB
Operations and data transfer prices

LRS	ZRS	GRS	RA-GRS
Queue Class 1* operations (in 10,000)	$0.004	$0.004	$0.008	$0.008
Queue Class 2** operations (in 10,000)	$0.004	$0.004	$0.004	$0.004
Geo-replication data transfer (per GB)	N/A	N/A	$0.02	$0.02
* The following Queue operations are counted as Class 1: CreateQueue, ListQueues, PutMessage, SetQueueMetadata, UpdateMessage, ClearMessages, DeleteMessage, DeleteQueue, GetMessageWrite, GetMessagesWrite

** The following Queue operations are counted as Class 2: GetMessage, GetMessages, GetQueueMetadata, GetQueueServiceProperties, GetQueueAcl, PeekMessage, PeekMessages, GetMessageRead, GetMessagesRead

