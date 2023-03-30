using FuncApp.DataContext;
using FuncApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FuncApp
{
    public class OnQueueTriggerUpdateDatabase
    {
        private readonly AzureDbContext _azureDbContext;

        public OnQueueTriggerUpdateDatabase(AzureDbContext azureDbContext)
        {
            _azureDbContext = azureDbContext;
        }

        [FunctionName("OnQueueTriggerUpdateDatabase")]
        public void Run([QueueTrigger("SalesRequestInBound", Connection = "AzureWebJobsStorage")] SalesRequest myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            myQueueItem.Status = "Submitted";

            _azureDbContext.SalesRequests.Add(myQueueItem);
            _azureDbContext.SaveChanges();
        }
    }
}
