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

## Azure Files limits
For additional details on Azure Files limits, see Azure Files scalability and performance targets.

Resource	Target
Max size of a file share	5 TiB
Max size of a file in a file share	1 TiB
Max number of files in a file share	No limit
Max IOPS per share	1000 IOPS
Max number of stored access policies per file share	5
Maximum request rate per storage account	20,000 requests per second for files of any valid size3
Target throughput for single file share	Up to 60 MiB per second
Maximum open handles per file	2000 open handles
Maximum number of share snapshots	200 share snapshots

## Azure File Sync limits
Resource	Target	Hard limit
Storage Sync Services per subscription	15 Storage Sync Services	No
Sync groups per Storage Sync Service	30 sync groups	Yes
Registered servers per Storage Sync Service	99 servers	Yes
Cloud endpoints per Sync Group	1 cloud endpoint	Yes
Server endpoints per Sync Group	50 server endpoints	No
Server endpoints per server	33-99 server endpoints	Yes, but varies based on configuration
Endpoint size	4 TiB	No
File system objects (directories and files) per sync group	25 million objects	No
Maximum number of file system objects (directories and files) in a directory	200,000 objects	Yes
Maximum object (directories and files) name length	255 characters	Yes
Maximum object (directories and files) security descriptor size	4 KiB	Yes
File size	100 GiB	No
Minimum file size for a file to be tiered	64 KiB	Yes



Files Storage
------------------------------

Azure File offers fully managed file shares in the cloud, accessible via the industry standard Server Message Block (SMB) protocol. Azure File shares can be mounted concurrently by cloud or on-premises deployments of Windows, macOS, and Linux. Enable file sharing between applications running in your virtual machines using familiar Windows APIs or File REST API. Additionally, Azure File Sync allows caching and synchronization of Azure File shares on Windows Servers for local access.

Data storage prices

Below are prices for storing data in Azure File shares, shown as monthly charges per GB of data stored. These prices vary based on the redundancy option you select.

LRS	ZRS	GRS
$0.06 per GB	$0.075 per GB	$0.10 per GB
Operations and data transfer prices

For both SMB and REST operations, transaction costs are incurred against your Azure File share, covering operations such as enumerating a directory or reading a file. These prices vary based on the redundancy option you select.

LRS	ZRS	GRS
Put, Create Container Operations (per 10,000)	$0.015	$0.0188	$0.03
List Operations (per 10,000)	$0.015	$0.015	$0.015
All other operations except Delete, which is free (per 10,000)	$0.0015	$0.0015	$0.0015
Geo-Replication Data Transfer (per GB)	N/A	N/A	$0.02
Prices marked as N/A indicate that the redundancy option is not available in the selected region. Azure File does not support read-access geo-redundant storage (RA-GRS) in any region at this time. File shares in the RA-GRS storage account are charged at GRS prices. Note: File operations that result in data transfer outside of an Azure datacenter will incur additional outbound data transfer charges.
File Sync Prices

These are the costs of syncing your files from Windows servers to Azure File shares in the cloud. Learn more about Azure File Sync.

The total cost of Azure File Sync (AFS) services is determined by the number of servers that connect to the cloud endpoint (Azure File Share) plus the underlying costs of File storage (including storage and access costs) and outbound data transfer.
