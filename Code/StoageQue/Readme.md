Go through: https://docs.microsoft.com/en-us/azure/storage/common/storage-performance-checklist#subheading39

Storage queues are designed to support standard queuing scenarios, such as decoupling application components to increase scalability and tolerance for failures, load leveling, and building process workflows.

## Azure Queue storage limits
Resource	Target
Max size of single queue	500 TiB
Max size of a message in a queue	64 KiB
Max number of stored access policies per queue	5
Maximum request rate per storage account 20,000 messages per second assuming 1 KiB message size
Target throughput for single queue (1 KiB messages)	Up to 2000 messages per second
Maximum number of queues	Unlimited
Maximum number of concurrent clients	Unlimited

Azure storage ques DO NOT guarantee message order and has no "push style" onMessage 
callbacks like the service bus que does. And you cannot batch send que messages.
They do not support automatic deadlettering, but they DO support transaction logs and Minute Metrics: provides real-time metrics for availability, TPS, API call counts, error counts, and more, all in real time (aggregated per minute and reported within a few minutes from what just happened in production. For more information, see About Storage Analytics Metrics.
They do not support message autoforwarding, duplicate detection or fetching message sessions by ID.

Storage queues enable you to obtain a detailed log of all of the transactions executed against the queue, as well as aggregated metrics. Both of these options are useful for debugging and understanding how your application uses Storage queues. They are also useful for performance-tuning your application and reducing the costs of using queues.

Messages in Storage queues are typically first-in-first-out, but sometimes they can be out of order; for example, when a message's visibility timeout duration expires (for example, as a result of a client application crashing during processing). When the visibility timeout expires, the message becomes visible again on the queue for another worker to dequeue it. At that point, the newly visible message might be placed in the queue (to be dequeued again) after a message that was originally enqueued after it.

Storage queues provide support for updating message content. You can use this functionality for persisting state information and incremental progress updates into the message so that it can be processed from the last known checkpoint, instead of starting from scratch. With Service Bus queues, you can enable the same scenario through the use of message sessions. Sessions enable you to save and retrieve the application processing state (by using SetState and GetState).

To find "poison" messages in Storage queues, when dequeuing a message the application examines the DequeueCount property of the message. If DequeueCount is greater than a given threshold, the application moves the message to an application-defined "dead letter" queue.

With Storage queues, if the content of the message is not XML-safe, then it must be Base64 encoded. If you Base64-encode the message, the user payload can be up to 48 KB, instead of 64 KB.

## performance concerns
The bigger your message, the slower your que operates. Keep your key messages small!

## Receive behavior: 
Non-blocking (completes immediately if no new message is found)

## Lease/Lock duration	
30 seconds (default)
7 days (maximum) (You can renew or release a message lease using the UpdateMessage API.)

## Lease/Lock precision	
Message level
(each message can have a different timeout value, which you can then update as needed while processing the message, by using the UpdateMessage API)

Storage queues provide leases with the ability to extend the leases for messages. This allows the workers to maintain short leases on messages. Thus, if a worker crashes, the message can be quickly processed again by another worker. In addition, a worker can extend the lease on a message if it needs to process it longer than the current lease time.

Storage queues offer a visibility timeout that you can set upon the enqueuing or dequeuing of a message. In addition, you can update a message with different lease values at run-time, and update different values across messages in the same queue. Service Bus lock timeouts are defined in the queue metadata; however, you can renew the lock by calling the RenewLock method.

## you should consider using Storage queues when:
Your application must store over 80 GB of messages in a queue.
Your application wants to track progress for processing a message inside of the queue. This is useful if the worker processing a message crashes. A subsequent worker can then use that information to continue from where the prior worker left off.
You require server side logs of all of the transactions executed against your queues.
Features such as the 200 TB ceiling of Storage queues (more when you virtualize accounts) and unlimited queues make it an ideal platform for SaaS providers.

URL format: Queues are addressable using the following URL format:
http://< storage account >.queue.core.windows.net/< queue >

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

