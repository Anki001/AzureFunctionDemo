using FuncApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace FuncApp
{
    public static class OnSalesUploadWriteToQueue
    {
        [FunctionName("OnSalesUploadWriteToQueue")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Queue("SalesRequestInBound", Connection ="AzureWebJobsStorage")] IAsyncCollector<SalesRequest> salesRequestCollector,
            ILogger log)
        {
            log.LogInformation("Sales request received by OnSalesUploadWriteToQueue function");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SalesRequest data = JsonConvert.DeserializeObject<SalesRequest>(requestBody);

            await salesRequestCollector.AddAsync(data);

            string responseMessage = $"Sales request has been received for `{data.Name}`";

            return new OkObjectResult(responseMessage);
        }
    }
}
