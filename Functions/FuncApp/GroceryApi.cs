using FuncApp.DataContext;
using FuncApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace FuncApp
{
    public class GroceryApi
    {
        private readonly AzureDbContext _dbContext;
        public GroceryApi(AzureDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [FunctionName("CreateGrocery")]
        public async Task<IActionResult> CreateGrocery(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "grocerylist")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating grocery list items.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            GroceryItem_Upsert data = JsonConvert.DeserializeObject<GroceryItem_Upsert>(requestBody);

            var groceryItem = new GroceryItem
            {
                Name = data.Name
            };

            _dbContext.Add(groceryItem);
            _dbContext.SaveChanges();
            return new OkObjectResult(groceryItem);
        }

        [FunctionName("GetAllGrocery")]
        public async Task<IActionResult> GetAllGrocery(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "grocerylist")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get all grocery list items.");
            var result = await _dbContext.GroceryItems.ToListAsync();

            return new OkObjectResult(result);
        }

        [FunctionName("GetGroceryById")]
        public async Task<IActionResult> GetGroceryById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "grocerylist/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation($"Get grocery from list items with id {id}.");
            var groceryItem = await _dbContext.GroceryItems.FirstOrDefaultAsync(x => x.Id == id);

            if (groceryItem is null)
                return new NotFoundResult();

            return new OkObjectResult(groceryItem);
        }

        [FunctionName("UpdateGrocery")]
        public async Task<IActionResult> UpdateGrocery(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "grocerylist/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("Updating grocery list items.");

            var groceryItemFromDb = await _dbContext.GroceryItems.FirstOrDefaultAsync(x => x.Id == id);

            if (groceryItemFromDb is null)
                return new NotFoundResult();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            GroceryItem_Upsert updatedData = JsonConvert.DeserializeObject<GroceryItem_Upsert>(requestBody);

            if (!string.IsNullOrEmpty(updatedData.Name))
            {
                groceryItemFromDb.Name = updatedData.Name;

                _dbContext.Update(groceryItemFromDb);
                _dbContext.SaveChanges();
            }

            return new OkObjectResult(groceryItemFromDb);
        }

        [FunctionName("DeleteGrocery")]
        public async Task<IActionResult> DeleteGrocery(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "grocerylist/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("Deleting grocery item.");
            var groceryItemFromDb = await _dbContext.GroceryItems.FirstOrDefaultAsync(x => x.Id == id);

            if (groceryItemFromDb is null)
                return new NotFoundResult();

            _dbContext.GroceryItems.Remove(groceryItemFromDb);
            _dbContext.SaveChanges();

            return new OkResult();
        }
    }
}
