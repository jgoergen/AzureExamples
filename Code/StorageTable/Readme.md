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

TODO: https://docs.microsoft.com/en-us/azure/storage/common/storage-performance-checklist#subheading24

## Azure Table storage limits
Resource	Target
Max size of single table	500 TiB
Max size of a table entity	1 MiB
Max number of properties in a table entity	255 (including 3 system properties: PartitionKey, RowKey and Timestamp)
Max number of stored access policies per table	5
Maximum request rate per storage account	20,000 transactions per second (assuming 1 KiB entity size)
Target throughput for single table partition (1 KiB entities)	Up to 2000 entities per second

Table Storage
------------------------------

Tables offer NoSQL storage for unstructured and semi-structured data—ideal for web applications, address books, and other user data.

Data storage prices

STORAGE CAPACITY	LRS	GRS	RA-GRS	ZRS
First 1 terabyte (TB) / month	$0.07 per GB	$0.095 per GB	$0.12 per GB	$0.0875 per GB
Next 49 TB (1 to 50 TB) / month	$0.065 per GB	$0.08 per GB	$0.10 per GB	$0.0813 per GB
Next 450 TB (50 to 500 TB) / month	$0.06 per GB	$0.07 per GB	$0.09 per GB	$0.075 per GB
Next 500 TB (500 to 1,000 TB) / month	$0.055 per GB	$0.065 per GB	$0.08 per GB	$0.0688 per GB
Next 4,000 TB (1,000 to 5,000 TB) / month	$0.045 per GB	$0.06 per GB	$0.075 per GB	$0.0563 per GB
Over 5,000 TB / Month	Contact us	Contact us	Contact us	Contact us
Operations and data transfer prices

We charge $0.00036 per 10,000 transactions for tables. Any type of operation against the storage is counted as a transaction, including reads, writes, and deletes.

## Managing Concurrency in the File Service

The file service can be accessed using two different protocol endpoints – SMB and REST. The REST service does not have support for either optimistic locking or pessimistic locking and all updates will follow a last writer wins strategy. SMB clients that mount file shares can leverage file system locking mechanisms to manage access to shared files – including the ability to perform pessimistic locking. When an SMB client opens a file, it specifies both the file access and share mode. Setting a File Access option of "Write" or "Read/Write" along with a File Share mode of "None" will result in the file being locked by an SMB client until the file is closed. If REST operation is attempted on a file where an SMB client has the file locked the REST service will return status code 409 (Conflict) with error code SharingViolation.

When an SMB client opens a file for delete, it marks the file as pending delete until all other SMB client open handles on that file are closed. While a file is marked as pending delete, any REST operation on that file will return status code 409 (Conflict) with error code SMBDeletePending. Status code 404 (Not Found) is not returned since it is possible for the SMB client to remove the pending deletion flag prior to closing the file. In other words, status code 404 (Not Found) is only expected when the file has been removed. Note that while a file is in a SMB pending delete state, it will not be included in the List Files results.Also note that the REST Delete File and REST Delete Directory operations are committed atomically and do not result in pending delete state.
