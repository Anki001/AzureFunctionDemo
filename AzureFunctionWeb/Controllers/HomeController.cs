using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureFunctionWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace AzureFunctionWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly BlobServiceClient _blobServiceClient;

        public HomeController(ILogger<HomeController> logger,
            BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _blobServiceClient = blobServiceClient;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SalesRequest salesRequest, IFormFile file)
        {
            salesRequest.Id = Guid.NewGuid().ToString();
            var salesRequestJson = JsonSerializer.Serialize(salesRequest);

            using (var content = new StringContent(salesRequestJson, Encoding.UTF8, "application/json"))
            {
                // Call azure function and pass the content
                HttpResponseMessage response = await _httpClient.PostAsync("http://localhost:7058/api/OnSalesUploadWriteToQueue", content);
                string returnValue = await response.Content.ReadAsStringAsync();
            }

            if (file is not null)
            {
                var fileName = salesRequest.Id + Path.GetExtension(file.FileName);
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("functionsalesrep");
                var blobClient = containerClient.GetBlobClient(fileName);

                var blobHttpHeader = new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                };

                await blobClient.UploadAsync(file.OpenReadStream(), blobHttpHeader);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}