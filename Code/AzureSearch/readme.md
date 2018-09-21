go through: https://docs.microsoft.com/en-us/rest/api/searchservice/Naming-rules
go through: https://docs.microsoft.com/en-us/rest/api/searchservice/Supported-data-types
go through: https://docs.microsoft.com/en-us/azure/search/search-what-is-azure-search
go through: https://docs.microsoft.com/en-us/azure/search/search-import-data-dotnet
go through: https://docs.microsoft.com/en-us/azure/search/search-query-dotnet
go through: https://docs.microsoft.com/en-us/azure/search/search-indexer-tutorial
go through: https://docs.microsoft.com/en-us/azure/search/search-autocomplete-tutorial
go through: https://docs.microsoft.com/en-us/azure/search/search-sku-tier
go through: https://docs.microsoft.com/en-us/azure/search/search-capacity-planning
go through: https://docs.microsoft.com/en-us/azure/search/search-what-is-an-index
go through: https://docs.microsoft.com/en-us/azure/search/search-synonyms
go through: https://docs.microsoft.com/en-us/azure/search/search-howto-reindex
go through: https://docs.microsoft.com/en-us/azure/search/search-howto-concurrency
go through: https://docs.microsoft.com/en-us/azure/search/search-lucene-query-architecture

# Pricing
	FREE	BASIC	STANDARD S1	STANDARD S2	STANDARD S3
Storage	50 MB	2 GB per service	25 GB/partition
(max 300 GB documents per service)	100 GB/partition
(max 1.2 TB documents per service)	200 GB/partition
(max 2.4 TB documents per service)
Max indexes per service	3	5	50	200	200 or 1000/partition in high density2 mode
Scale out limits	N/A	Up to 3 units per service
(max 1 partition; max 3 replicas)	Up to 36 units per service
(max 12 partitions; max 12 replicas)	Up to 36 units per service
(max 12 partitions; max 12 replicas)	Up to 36 units per service
(max 12 partitions; max 12 replicas)
up to 12 replicas in high density2 mode
Data transfer	Standard rates apply	Standard rates apply	Standard rates apply	Standard rates apply	Standard rates apply
Price per unit	Free	$0.101/hour	$0.336/hour	$1.344/hour	$2.688/hour

The stop button only stops traffic, it does not stop you from being charged!!
Billing is rounded to the hour!!

## search index name limitations
Index name must only contain lowercase letters, digits or dashes, cannot start or end with dashes and is limited to 128 characters.

## note on building classes for working with index entries
When designing your own model classes to map to an Azure Search index, we recommend declaring properties of value types such as bool and int to be nullable (for example, bool? instead of bool). If you use a non-nullable property, you have to guarantee that no documents in your index contain a null value for the corresponding field. Neither the SDK nor the Azure Search service will help you to enforce this.  

# Azure Search Indexers  
If you are using the Azure Search Indexer, you are already importing data changes from a central datastore such as Azure SQL DB or Azure Cosmos DB. When you create a new search Service, you simply also create a new Azure Search Indexer for that service that points to this same datastore. That way, whenever new changes come into the data store, they will then be indexed by the various Indexers.

# Push API  
If you are using the Azure Search Push API to update content in your Azure Search index, you can keep your various search services in sync by pushing changes to all search services whenever an update is required. When doing this it is important to make sure to handle cases where an update to one search service fails and one or more updates succeed.  

# Facets  
Facets are dynamic and returned on a query. Search responses bring with them the facet categories used to navigate the results.  
Faceting is enabled on a field-by-field basis when you create the index, by setting the following attributes to TRUE: filterable, facetable. Only filterable fields can be faceted.  
A facet is a query parameter, but do not confuse it with query input. It is never used as selection criteria in a query. Instead, think of facet query parameters as inputs to the navigation structure that comes back in the response. For each facet query parameter you provide, Azure Search evaluates how many documents are in the partial results for each facet value.  
A facet query parameter is set to a field and depending on the data type, can be further parameterized by comma-delimited list that includes count:<integer>, sort:<>, interval:<integer>, and values:<list>.   

## Limit the number of items in the facet navigation  
For each faceted field in the navigation tree, there is a default limit of 10 values. This default makes sense for navigation structures because it keeps the values list to a manageable size. You can override the default by assigning a value to count.  
'&facet=city,count:5': specifies that only the first five cities found in the top ranked results are returned as a facet result. Consider a sample query with a search term of “airport” and 32 matches. If the query specifies &facet=city,count:5, only the first five unique cities with the most documents in the search results are included in the facet results.  

## Get counts in facet results  
When you add a filter to a faceted query, you might want to retain the facet statement (for example, facet=Rating&$filter=Rating ge 4). Technically, facet=Rating isn’t needed, but keeping it returns the counts of facet values for ratings 4 and higher. For example, if you click "4" and the query includes a filter for greater or equal to "4", counts are returned for each rating that is 4 and higher.  

# Search and Filters

## Filters use the OData $filter expression
Azure Search supports a subset of the OData expression syntax for $filter and $orderby expressions.  
References to field names. Only filterable fields can be used in filter expressions.
Examples:  
1) 'baseRate lt 150'  
3) 'price ge 60 and price lt 300'  
4) '(baseRate ge 60 and baseRate lt 300) or hotelName eq "Fancy Stay"'  
5) 'baseRate lt 100.0 and rating ge 4 ': Find all hotels with a base rate less than $100 that are rated at or above 4:  
6) 'hotelName ne 'Roach Motel' and lastRenovationDate ge 2010-01-01T00:00:00Z': Find all hotels other than "Roach Motel" that have been renovated since 2010:  
7) 'parkingIncluded and not smokingAllowed ': Find all hotels that have parking included and do not allow smoking  
8) 'tags/all(t: t ne 'motel')':  Find all hotels without the tag "motel"  
9) 'geo.distance(location, geography'POINT(-122.131577 47.678581)') le 10': Find all hotels within 10 kilometers of a given reference point (where location is a field of type Edm.GeographyPoint)  
10) 'search.in(name, 'Roach motel,Budget hotel', ',')':  Find all hotels with name equal to either Roach motel' or 'Budget hotel')  
11) 'not search.ismatch('luxury')': Find documents without the word "luxury".  
12) 'baseRate asc': Sort hotels ascending by base rate:  
13) 'rating desc,geo.distance(location, geography'POINT(-122.131577 47.678581)') asc': Sort hotels descending by rating, then ascending by distance from the given co-ordinates:  

## Filter operators
Logical operators (and, or, not).  

Comparison expressions (eq, ne, gt, lt, ge, le). String comparisons are case-sensitive.  

any with no parameters. This tests whether a field of type Collection(Edm.String) contains any elements.  
any/all are supported on fields of type Collection(Edm.String).  
any can only be used with simple equality expressions or with a search.in function.  
Simple expressions consist of a comparison between a single field and a literal value, e.g. Title eq 'Magna Carta'.  
all can only be used with simple inequality expressions or with a not search.in.  

Geospatial functions geo.distance and geo.intersects.  
The geo.distance function returns the distance in kilometers between two points, one being a field and one being a constant passed as part of the filter. The geo.intersects function returns true if a given point is within a given polygon, where the point is a field and the polygon is specified as a constant passed as part of the filter.  
The polygon is a two-dimensional surface stored as a sequence of points defining a bounding ring (see the example below). The polygon needs to be closed, meaning the first and last point sets must be the same. Points in a polygon must be in counterclockwise order.  
geo.distance returns distance in kilometers in Azure Search. This differs from other services that support OData geospatial operations, which typically return distances in meters.  

Note:  
When using geo.distance in a filter, you must compare the distance returned by the function with a constant using lt, le, gt, or ge. The operators eq and ne are not supported when comparing distances. For example, this is a correct usage of geo.distance: $filter=geo.distance(location, geography'POINT(-122.131577 47.678581)') le 5.  

The search.in function tests whether a given string field is equal to one of a given list of values. It can also be used in any or all to compare a single value of a string collection field with a given list of values. Equality between the field and each value in the list is determined in a case-sensitive fashion, the same way as for the eq operator. Therefore an expression like search.in(myfield, 'a, b, c') is equivalent to myfield eq 'a' or myfield eq 'b' or myfield eq 'c', except that search.in will yield much better performance.  
The first parameter to the search.in function is the string field reference (or a range variable over a string collection field in the case where search.in is used inside an any or all expression). The second parameter is a string containing the list of values, separated by spaces and/or commas. If you need to use separators other than spaces and commas because your values include those characters, you can specify an optional third parameter to search.in.  
This third parameter is a string where each character of the string, or subset of this string is treated as a separator when parsing the list of values in the second parameter.  

Note:  
Some scenarios require comparing a field against a large number of constant values. For example, implementing security trimming with filters might require comparing the document ID field against a list of IDs to which the requesting user is granted read access. In scenarios like this we highly recommend using the search.in function instead of a more complicated disjunction of equality expressions. For example, use search.in(Id, '123, 456, ...') instead of Id eq 123 or Id eq 456 or .....  

The search.ismatch function evaluates search query as a part of a filter expression. The documents that match the search query will be returned in the result set. The following overloads of this function are available:  

search.ismatch(search)  
search.ismatch(search, searchFields)  
search.ismatch(search, searchFields, queryType, searchMode)  

where:
search: the search query (in either simple or full query syntax).  
queryType: "simple" or "full", defaults to "simple". Specifies what query language was used in the search parameter.  
searchFields: comma-separated list of searchable fields to search in, defaults to all searchable fields in the index.  
searchMode: "any" or "all", defaults to "any". Indicates whether any or all of the search terms must be matched in order to count the document as a match.  

## Filter size limitations
There are limits to the size and complexity of filter expressions that you can send to Azure Search. The limits are based roughly on the number of clauses in your filter expression. A good rule of thumb is that if you have hundreds of clauses, you are at risk of running into the limit. We recommend designing your application in such a way that it does not generate filters of unbounded size.  

## Full Search expressions us Lucene expressions
Examples:  
1) Proximity search: '"hotel airport"~5': will find the terms "hotel" and "airport" within 5 words of each other in a document.  
2) genre: 'jazz NOT history'  
3) artists:("Miles Davis" "John Coltrane")  
4) Term boosting: 'rock^2 electronic':  boost documents that contain the search terms in the genre field higher than other searchable fields in the index  
5) Regular expression search: '/[mh]otel/'  
6) Wildcard search: 'Spacious, air-condition* +\"Ocean view\"'    
7) Precedence operators: grouping and field grouping: 'motel+(wifi||luxury)': will search for documents containing the "motel" term and either "wifi" or "luxury" (or both).   
8) 'hotelAmenities:(gym+(wifi||pool))': searches the field "hotelAmenities" for "gym" and "wifi", or "gym" and "pool".  

## Escaping special characters
Special characters must be escaped to be used as part of the search text. You can escape them by prefixing them with backslash (\). Special characters that need to be escaped include the following:  
+ - && || ! ( ) { } [ ] ^ " ~ * ? : \ /  

## Operators in Lucene Search
You can embed operators in a query string to build a riche set of criteria against which matching documents are found.  

AND operator +: The AND operator is a plus sign. For example, wifi+luxury will search for documents containing both wifi and luxury.  

OR operator |: The OR operator is a vertical bar or pipe character. For example, wifi | luxury will search for documents containing either wifi or luxury or both.  

NOT operator -: The NOT operator is a minus sign. For example, wifi –luxury will search for documents that have the wifi term and/or do not have luxury (and/or is controlled by searchMode).  

The searchMode option controls whether a term with the NOT operator is ANDed or ORed with the other terms in the query in the absence of a + or | operator. Recall that searchMode can be set to either any (default) or all. If you use any, it will increase the recall of queries by including more results, and by default - will be interpreted as "OR NOT". For example, wifi -luxury will match documents that either contain the term wifi or those that do not contain the term luxury. If you use all, it will increase the precision of queries by including fewer results, and by default - will be interpreted as "AND NOT". For example, wifi -luxury will match documents that contain the term wifi and do not contain the term "luxury". This is arguably a more intuitive behavior for the - operator. Therefore, you should consider using searchMode=all instead of searchMode=any if You want to optimize searches for precision instead of recall, and Your users frequently use the - operator in searches.  

Suffix operator *: The suffix operator is an asterisk. For example, lux* will search for documents that have a term that starts with lux, ignoring case.  

Phrase search operator " ": The phrase operator encloses a phrase in quotation marks. For example, while Roach Motel (without quotes) would search for documents containing Roach and/or Motel anywhere in any order, "Roach Motel" (with quotes) will only match documents that contain that whole phrase together and in that order (text analysis still applies).  

Precedence operator ( ): The precedence operator encloses the string in parentheses. For example, motel+(wifi | luxury) will search for documents containing the motel term and either wifi or luxury (or both).|  

## Escaping search operators
In order to use the above symbols as actual part of the search text, they should be escaped by prefixing them with a backslash. For example, luxury\+hotel will result in the term luxury+hotel. In order to make things simple for the more typical cases, there are two exceptions to this rule where escaping is not needed:  

The NOT operator - only needs to be escaped if it's the first character after whitespace, not if it's in the middle of a term. For example, wi-fi is a single term; whereas GUIDs (such as 3352CDD0-EF30-4A2E-A512-3B30AF40F3FD) are treated as a single token.  

The suffix operator * needs to be escaped only if it's the last character before whitespace, not if it's in the middle of a term. For example, wi*fi is treated as a single token.  

# When optimizing for search latency it is important to:  
1) Pick a target latency (or maximum amount of time) that a typical search request should take to complete.  
2) Create and test a real workload against your search service with a realistic dataset to measure these latency rates.  
3) Start with a low number of queries per second (QPS) and continue to increase the number executed in the test until the query latency drops below the defined target latency. This is an important benchmark to help you plan for scale as your application grows in usage.  
4) Wherever possible, reuse HTTP connections. If you are using the Azure Search .NET SDK, this means you should reuse an instance or SearchIndexClient instance, and if you are using the REST API, you should reuse a single HttpClient.  

While creating these test workloads, there are some characteristics of Azure Search to keep in mind:  
1) It is possible to push so many search queries at one time, that the resources available in your Azure Search service will be overwhelmed. When this happens, you will see HTTP 503 response codes. For this reason, it is best to start with various ranges of search requests to see the differences in latency rates as you add more search requests.  
2) Uploading of content to Azure Search will impact the overall performance and latency of the Azure Search service. If you expect to send data while users are performing searches, it is important to take this workload into account in your tests.  
3) Not every search query will perform at the same performance levels. For example, a document lookup or search suggestion will typically perform faster than a query with a significant number of facets and filters. It is best to take the various queries you expect to see into account when building your tests.  
4) Variation of search requests is important because if you continually execute the same search requests, caching of data will start to make performance look better than it might with a more disparate query set.  

# Scaling Azure Search for high query rates and throttled requests  
When you are receiving too many throttled requests or exceed your target latency rates from an increased query load, you can look to decrease latency rates in one of two ways:  
1) Increase Replicas: A replica is like a copy of your data allowing Azure Search to load balance requests against the multiple copies. All load balancing and replication of data across replicas is managed by Azure Search and you can alter the number of replicas allocated for your service at any time. You can allocate up to 12 replicas in a Standard search service and 3 replicas in a Basic search service. Replicas can be adjusted either from the Azure portal or PowerShell.  
2) Increase Search Tier: Azure Search comes in a number of tiers and each of these tiers offers different levels of performance. In some cases, you may have so many queries that the tier you are on cannot provide sufficiently low latency rates, even when replicas are maxed out. In this case, you may want to consider leveraging one of the higher search tiers such as the Azure Search S3 tier that is well suited for scenarios with large numbers of documents and extremely high query workloads.  

# Scaling Azure Search for slow individual queries  
Another reason why latency rates can be slow is from a single query taking too long to complete. In this case, adding replicas will not improve latency rates. For this case there are two options available:  

1) Increase Partitions A partition is a mechanism for splitting your data across extra resources. For this reason, when you add a second partition, your data gets split into two. A third partition splits your index into three, etc. This also has the effect that in some cases, slow queries will perform faster due to the parallelization of computation. There are a few examples of where we have seen this parallelization work extremely well with queries that have low selectivity queries. This consists of queries that match many documents or when faceting needs to provide counts over large numbers of documents. Since there is a lot of computation needed to score the relevancy of the documents or to count the numbers of documents, adding extra partitions can help to provide additional computation.  

There can be a maximum of 12 partitions in Standard search service and 1 partition in the basic search service. Partitions can be adjusted either from the Azure portal or PowerShell.  

1) Limit High Cardinality Fields: A high cardinality field consists of a facetable or filterable field that has a significant number of unique values, and as a result, takes a lot of resources to compute results over. For example, setting a Product ID or Description field as facetable/filterable would make for high cardinality because most of the values from document to document are unique. Wherever possible, limit the number of high cardinality fields.  
2) Increase Search Tier: Moving up to a higher Azure Search tier can be another way to improve performance of slow queries. Each higher tier also provides faster CPU’s and more memory which can have a positive impact on query performance.  

