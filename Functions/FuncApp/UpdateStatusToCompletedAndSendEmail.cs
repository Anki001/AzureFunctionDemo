using System;
using System.Collections.Generic;
using System.Linq;
using FuncApp.DataContext;
using FuncApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FuncApp
{
    public class UpdateStatusToCompletedAndSendEmail
    {
        private readonly AzureDbContext _azureDbContext;

        public UpdateStatusToCompletedAndSendEmail(AzureDbContext azureDbContext)
        {
            _azureDbContext = azureDbContext;
        }

        [FunctionName("UpdateStatusToCompletedAndSendEmail")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            IEnumerable<SalesRequest> salesRequestFromDb = _azureDbContext.SalesRequests
                .Where(x => x.Status == "Image Processed");

            if (salesRequestFromDb.Any())
            {
                foreach (var item in salesRequestFromDb)
                {
                    item.Status = "Completed";
                }
                _azureDbContext.UpdateRange(salesRequestFromDb);
                _azureDbContext.SaveChanges();
                log.LogInformation($"Status updated to Completed at: {DateTime.Now}");

                // Write logic to send email
                log.LogInformation($"Email sent successfully at: {DateTime.Now}");
            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
