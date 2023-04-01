using FuncApp.DataContext;
using FuncApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;

namespace FuncApp
{
    public class OnBlobResizeUpdateStatusInDb
    {
        private readonly AzureDbContext _azureDbContext;

        public OnBlobResizeUpdateStatusInDb(AzureDbContext azureDbContext)
        {
            _azureDbContext = azureDbContext;
        }

        [FunctionName("OnBlobResizeUpdateStatusInDb")]
        public void Run([BlobTrigger("functionsalesrep-sm/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            var fileName = Path.GetFileNameWithoutExtension(name);
            SalesRequest salesRequestFromBd = _azureDbContext.SalesRequests
                .FirstOrDefault(u => u.Id == fileName);

            if (salesRequestFromBd is not null)
            {
                salesRequestFromBd.Status = "Image Processed";
                _azureDbContext.SalesRequests.Update(salesRequestFromBd);
                _azureDbContext.SaveChanges();
            }
        }
    }
}
