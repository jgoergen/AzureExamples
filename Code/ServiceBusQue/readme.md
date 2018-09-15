https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-performance-improvements
https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dotnet-get-started-with-queues
https://www.codemag.com/article/1112041
http://www.cloudcasts.net/devguide/Default.aspx?id=11030

## Azure Service bus Queue storage limits
Maximum queue size	1 GB to 80 GB (defined upon creation of a queue and enabling partitioning – see the “Additional Information” section)
Maximum message size	256 KB or 1 MB (including both header and body, maximum header size: 64 KB). Depends on the service tier.
Maximum message TTL		TimeSpan.Max
Maximum number of queues	10,000 (per service namespace)
Maximum number of concurrent clients	Unlimited (100 concurrent connection limit only applies to TCP protocol-based communicati

## you should consider using Service Bus queues when:
Your solution must be able to receive messages without having to poll the queue. With Service Bus, this can be achieved through the use of the long-polling receive operation using the TCP-based protocols that Service Bus supports.
Your solution requires the queue to provide a guaranteed first-in-first-out (FIFO) ordered delivery.
Your solution must be able to support automatic duplicate detection.
You want your application to process messages as parallel long-running streams (messages are associated with a stream using the SessionId property on the message). In this model, each node in the consuming application competes for streams, as opposed to messages. When a stream is given to a consuming node, the node can examine the state of the application stream state using transactions.
Your solution requires transactional behavior and atomicity when sending or receiving multiple messages from a queue.
Your application handles messages that can exceed 64 KB but will not likely approach the 256 KB limit.
You deal with a requirement to provide a role-based access model to the queues, and different rights/permissions for senders and receivers.
Your queue size will not grow larger than 80 GB.
You want to use the AMQP 1.0 standards-based messaging protocol. For more information about AMQP, see Service Bus AMQP Overview.
You can envision an eventual migration from queue-based point-to-point communication to a message exchange pattern that enables seamless integration of additional receivers (subscribers), each of which receives independent copies of either some or all messages sent to the queue. The latter refers to the publish/subscribe capability natively provided by Service Bus.
Your messaging solution must be able to support the "At-Most-Once" delivery guarantee without the need for you to build the additional infrastructure components.
You would like to be able to publish and consume batches of messages.

Service Bus enforces queue size limits. The maximum queue size is specified upon creation of the queue and can have a value between 1 and 80 GB. If the queue size value set on creation of the queue is reached, additional incoming messages will be rejected and an exception will be received by the calling code. For more information about quotas in Service Bus, see Service Bus Quotas.

Partitioning is not supported in the Premium tier. In the Standard tier, you can create Service Bus queues in 1, 2, 3, 4, or 5 GB sizes (the default is 1 GB). In Standard tier, with partitioning enabled (which is the default), Service Bus creates 16 partitions for each GB you specify. As such, if you create a queue that is 5 GB in size, with 16 partitions the maximum queue size becomes (5 * 16) = 80 GB. You can see the maximum size of your partitioned queue or topic by looking at its entry on the Azure portal.

Receive behavior: 
Blocking with/without timeout (offers long polling, or the "Comet technique")
Non-blocking (through the use of .NET managed API only)

Dead lettering, which is only supported by Service Bus queues, can be useful for isolating messages that cannot be processed successfully by the receiving application or when messages cannot reach their destination due to an expired time-to-live (TTL) property. The TTL value specifies how long a message remains in the queue. With Service Bus, the message will be moved to a special queue called $DeadLetterQueue when the TTL period expires.

The concept of "message sessions" supported by Service Bus enables messages that belong to a certain logical group to be associated with a given receiver, which in turn creates a session-like affinity between messages and their respective receivers. You can enable this advanced functionality in Service Bus by setting the SessionID property on a message. Receivers can then listen on a specific session ID and receive messages that share the specified session identifier.

The duplication detection functionality supported by Service Bus queues automatically removes duplicate messages sent to a queue or topic, based on the value of the MessageId property.

With Service Bus queues, each message stored in a queue is composed of two parts: a header and a body. The total size of the message cannot exceed the maximum message size supported by the service tier.

When clients communicate with Service Bus queues over the TCP protocol, the maximum number of concurrent connections to a single Service Bus queue is limited to 100. This number is shared between senders and receivers. If this quota is reached, subsequent requests for additional connections will be rejected and an exception will be received by the calling code. This limit is not imposed on clients connecting to the queues using REST-based API.

If you require more than 10,000 queues in a single Service Bus namespace, you can contact the Azure support team and request an increase. To scale beyond 10,000 queues with Service Bus, you can also create additional namespaces using the Azure portal.

## Lease/Lock duration	
60 seconds (default)
You can renew a message lock using the RenewLock API.

## Lease/Lock precision	
Queue level
(each queue has a lock precision applied to all of its messages, but you can renew the lock using the RenewLock API.)

Queue auto-forwarding enables thousands of queues to auto-forward their messages to a single queue, from which the receiving application consumes the message. You can use this mechanism to achieve security, control flow, and isolate storage between each message publisher.

