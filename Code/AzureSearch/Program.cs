using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Newtonsoft.Json;
// you will have to include a reference to System.ComponentModel.DataAnnotations
using System.ComponentModel.DataAnnotations;

namespace AzureSearch
{
    class Program
    {
        private static string SEARCH_SERVICE_NAME = "learningsearch";
        private static string API_KEY = "4C17B28DEE0D72A2FE1A329D8193BF11";
        // 'Index name must only contain lowercase letters, digits or dashes, cannot start or end with dashes and is limited to 128 characters.'
        private static string SEARCH_INDEX_NAME = "hotels";

        private static SearchServiceClient serviceClient;
        private static ISearchIndexClient indexClient;

        // TODO: make a demo for 'prefix' matching, pulling back items with XXX in the name

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Azure Search Examples");
            Console.WriteLine("Connecting to Storage Search");

            // note: this can be either an admin key or a query key, in this case we'll use admin
            // but we'd prefer to use query if all we're doing is reading data.

            // Wherever possible, reuse HTTP connections. If you are using the Azure Search .NET SDK, this means you should reuse 
            // an instance or SearchIndexClient instance, and if you are using the REST API, you should reuse a single HttpClient.
            serviceClient = new SearchServiceClient(SEARCH_SERVICE_NAME, new SearchCredentials(API_KEY));

            Console.WriteLine($"Creating an Index Client");

            indexClient = new SearchIndexClient(
                   SEARCH_SERVICE_NAME,
                   SEARCH_INDEX_NAME,
                   new SearchCredentials(API_KEY));

            // note: you can also create a client from an already existing index
            // indexClient = serviceClient.Indexes.GetClient(SEARCH_INDEX_NAME);

            // run tests
            Console.WriteLine("Starting example snippets\r\n");
            DeleteIndexIfExists();
            CreateIndex();
            CreateIndexEntries();
            SearchEntireIndexByPhrase();
            SearchIndexByRate();
            SearchIndexByName();
            SearchIndexByNameWithFacets();

            // paging
            // upload csv data from blob
            // IndexBatch.Merge
            // IndexBatch.MergeOrUpload
            // IndexBatch.Delete 
            // IndexBatch.New

            Console.WriteLine($"Examples completed, press enter to quit.");
            Console.ReadLine();
        }

        static void DeleteIndexIfExists()
        {
            Console.WriteLine($"Starting Delete Index if Exists Snippet");

            if (serviceClient.Indexes.Exists(SEARCH_INDEX_NAME))
            {
                Console.WriteLine($"Index exists, deleting it");
                serviceClient.Indexes.Delete(SEARCH_INDEX_NAME);
            }

            Console.WriteLine($"Finished Delete Index if Exists Snippet\r\n");
        }

        static void CreateIndex()
        {
            Console.WriteLine($"Starting Create Index Snippet");

            // note the hotel class, below
            var definition = new Index()
            {
                Name = SEARCH_INDEX_NAME,
                Fields = FieldBuilder.BuildForType<Hotel>()
            };

            serviceClient.Indexes.Create(definition);

            Console.WriteLine($"Finished Create Index Snippet\r\n");
        }

        static void CreateIndexEntries()
        {
            Console.WriteLine($"Starting Create Index Entries Snippet");

            var hotels = new Hotel[]
            {
                new Hotel()
                {
                    Id = "1",
                    Rate = 249.0,
                    Description = "Best hotel in town",
                    Name = "Fancy Stay",
                    Tags = new[] { "pool", "view", "wifi", "concierge" },
                    Location = GeographyPoint.Create(47.678581, -122.131577)
                },
                new Hotel()
                {
                    Id = "2",
                    Rate = 79.99,
                    Description = "Cheapest hotel in town",
                    Name = "Roach Motel",
                    Tags = new[] { "motel", "budget" },
                    Location = GeographyPoint.Create(49.678581, -122.131577)
                },
                new Hotel()
                {
                    Id = "3",
                    Rate = 129.99,
                    Description = "Close to town hall and the river"
                }
            };

            var batch = IndexBatch.Upload(hotels);

            try
            {
                indexClient.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
                // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                // the batch. Depending on your application, you can take compensating actions like delaying and retrying
                Console.WriteLine(
                    "Failed to index some of the documents: {0}",
                    String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));

                Console.WriteLine("Trying to index the failed entries one more time.");

                // This should be repeating and exponentially increasing, i'm being lazy for this exampel
                Thread.Sleep(5000);

                // get the failed entries
                // note: the lamba expression here correlates the entity with it's key field
                var retryBatch = e.FindFailedActionsToRetry<Hotel>(batch, hotel => hotel.Id);
                indexClient.Documents.Index(retryBatch);
            }

            // Indexing happens asynchronously in your Azure Search service, so the sample application needs to wait a short time to 
            // ensure that the documents are available for searching. Delays like this are typically only necessary in demos, tests, 
            // and sample applications.
            Console.WriteLine("Waiting for documents to be indexed...\n");
            Thread.Sleep(2000);

            Console.WriteLine($"Finished Create Index Entries Snippet");
        }

        static void SearchIndexByName()
        {
            Console.WriteLine($"Starting Search Index by Name Snippet");

            SearchParameters parameters;
            DocumentSearchResult<Hotel> results;

            Console.WriteLine("Search the entire index for the term 'budget' and return only the Name field:");

            parameters =
                new SearchParameters()
                {
                    // note 'name' is lowercase, even though it's PascalCase in our class.
                    // these will ALWAYS be camelCase in the index!
                    Select = new[] { "name" }
                };

            results = indexClient.Documents.Search<Hotel>("budget", parameters);

            foreach (SearchResult<Hotel> result in results.Results)
            {
                Console.WriteLine(result.Document.Name);
            }

            Console.WriteLine($"Finished Search Index by Name Snippet\r\n");
        }

        static void SearchIndexByNameWithFacets()
        {
            Console.WriteLine($"Starting Search Index by Name with Facets Snippet");

            SearchParameters parameters;
            DocumentSearchResult<Hotel> results;

            Console.WriteLine("Search the entire index for the term 'budget' and return only the Name field:");

            parameters =
                new SearchParameters()
                {
                    // note 'name' is lowercase, even though it's PascalCase in our class.
                    // these will ALWAYS be camelCase in the index!
                    Select = new[] { "name" },
                    Facets = new List<String>() { "tags", "rate,interval:50" }
                };

            results = indexClient.Documents.Search<Hotel>("*", parameters);

            foreach (SearchResult<Hotel> result in results.Results)
            {
                Console.WriteLine(result.Document.Name);
            }

            Console.WriteLine("Facets returned:");

            var facets = results.Facets.Values.ToList();
            var keys = results.Facets.Keys.ToList();

            for (var fsindex = 0; fsindex < facets.Count; fsindex ++)
                for (var findex = 0; findex < facets[fsindex].Count; findex ++)
                    Console.WriteLine($"  {keys[fsindex]} {facets[fsindex][findex].Value}");

            Console.WriteLine("\r\nRe-Search but using a 'selected' facet, this time");

            parameters =
                new SearchParameters()
                {
                    // note 'name' is lowercase, even though it's PascalCase in our class.
                    // these will ALWAYS be camelCase in the index!
                    Select = new[] { "name" },
                    Facets = new List<String>() { "tags" },
                    // note that tags has "IsSearchable" which is why we can use search.ismatch
                    Filter = "search.ismatch('budget', 'tags')"
                };

            results = indexClient.Documents.Search<Hotel>("*", parameters);

            foreach (SearchResult<Hotel> result in results.Results)
            {
                Console.WriteLine(result.Document.Name);
            }

            Console.WriteLine($"Finished Search Index by Name with Facets Snippet\r\n");
        }

        static void SearchIndexByRate() 
        {
            Console.WriteLine($"Starting Search Index by Rate Snippet");

            SearchParameters parameters;
            DocumentSearchResult<Hotel> results;

            Console.Write("Apply a filter to the index to find hotels cheaper than $150 per night, ");
            Console.WriteLine("and return the hotelId and description:\r\n");

            parameters =
                new SearchParameters()
                {
                    // note 'rate, id, name, description, rate' are lowercase, even though they're PascalCase in our class.
                    // these will ALWAYS be camelCase in the index!
                    Filter = "rate lt 150",
                    Select = new[] { "id", "name", "description", "rate" }
                };

            results = indexClient.Documents.Search<Hotel>("*", parameters);

            foreach (SearchResult<Hotel> result in results.Results)
            {
                Console.WriteLine($"{result.Document.Name} {result.Document.Description} {result.Document.Rate}");
            }

            Console.WriteLine($"Finished Search Index by Rate Snippet\r\n");
        }

        static void SearchEntireIndexByPhrase()
        {
            Console.WriteLine($"Starting Search Entire Index by Phrase Snippet");

            SearchParameters parameters;
            DocumentSearchResult<Hotel> results;

            Console.WriteLine("Search the entire index for the term 'motel':\n");

            parameters = new SearchParameters();
            results = indexClient.Documents.Search<Hotel>("motel", parameters);

            foreach (SearchResult<Hotel> result in results.Results)
            {
                Console.WriteLine(result.Document.Name);
            }

            Console.WriteLine($"Finished Search Entire Index by Phrase  Snippet\r\n");
        }
    }

    // When designing your own model classes to map to an Azure Search index, we recommend declaring properties of value 
    // types such as bool and int to be nullable(for example, bool? instead of bool). If you use a non-nullable property, 
    // you have to guarantee that no documents in your index contain a null value for the corresponding field.Neither the 
    // SDK nor the Azure Search service will help you to enforce this.

    // The SerializePropertyNamesAsCamelCase attribute is defined in the Azure Search .NET SDK.
    // It ensures that Pascal-case property names in the model class are mapped to camel-case
    // field names in the index.
    [SerializePropertyNamesAsCamelCase]
    public partial class Hotel
    {
        [Key]
        [IsFilterable]
        public string Id { get; set; }

        [IsFilterable, IsSortable, IsFacetable]
        public double? Rate { get; set; }

        [IsSearchable]
        public string Description { get; set; }

        [IsSearchable, IsFilterable, IsSortable]
        public string Name { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        public string[] Tags { get; set; }

        [IsFilterable, IsSortable]
        public GeographyPoint Location { get; set; }
    }
}
