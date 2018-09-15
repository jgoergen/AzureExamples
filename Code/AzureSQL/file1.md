## The difference between IaaS and PaaS uses of SQL in Azure
IaaS: Strait up Azure SQL
    No management
    Built in backups
    Billed hourly
    No jobs
    no linked servers
    no filestream
    no windows integrated authentication
Paas: Just use a VM and install full SQL on it.
    You have to patch, manage it yourself
    You have to get your own license for it
    You have to manage backups, patching, etc.    

You can change the 'size' or tier anytime you want ( less then 4 seconds to switch on average )

DTU's ROUGHLY translate to about X transactions per second. You can find out a better estimate using the Azure SQL Database DTU Calculator


## Elastic Database pools
    Multiple databases that all share a common set of resources which is cheaper if you have a bunch of dbs that are often not doing much
    All databases will be limited by your tier limits together. Singles outside of an elastic pool would be limited individually
    Uses "eDTUs" 

Azure SQL Servers are more like containers, then you add databases to them which are more like microservices.

    
