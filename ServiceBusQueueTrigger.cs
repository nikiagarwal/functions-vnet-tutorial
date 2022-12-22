using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Logging;
using Azure;
using System.Threading.Tasks;

namespace Company.Function
{
    public static class ServiceBusQueueTrigger
    {
        [FunctionName("ServiceBusQueueTrigger")]
        public static async Task RunAsync([ServiceBusTrigger("queue", Connection = "SERVICEBUS_CONNECTION")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=pvtteststoragenonfunc;AccountKey=Ww20hoD55T7SVGcZ3W6rK/RJ5naytm2olusxo97728H48Du5dp3LyE+vo898InemKmKV0nF2kUGX+AStT2EH+A==;EndpointSuffix=core.windows.net");
            await ListBlobsFlatListing(blobServiceClient, 2, log);

        }
        
        private static async Task ListBlobsFlatListing(BlobServiceClient blobServiceClient,
                                               int? segmentSize, ILogger log)
        {
            try
            {

                // Get a reference to a container named "sample-container" and then create it
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("test");

                // Call the listing operation and return pages of the specified size.
                var resultSegment = containerClient.GetBlobsAsync()
                    .AsPages(default, segmentSize);

                // Enumerate the blobs returned for each page.
                await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
                {
                    foreach (BlobItem blobItem in blobPage.Values)
                    {
                        Console.WriteLine("Blob name: {0}", blobItem.Name);
                        log.LogInformation("Blob name: {0}", blobItem.Name);

                    }

                    Console.WriteLine();
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                log.LogInformation("Blob exception", e.Message);
                Console.ReadLine();
                throw;
            }
        }

    }

    
}
