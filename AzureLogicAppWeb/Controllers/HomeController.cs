using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureLogicAppWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace AzureLogicAppWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger,
            BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
            _httpClient = new HttpClient();
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(SpookyRequest request, IFormFile file)
        {
            request.Id = Guid.NewGuid().ToString();
            var salesRequestJson = JsonSerializer.Serialize(request);

            using (var content = new StringContent(salesRequestJson, Encoding.UTF8, "application/json"))
            {
                // Call azure function and pass the content
                HttpResponseMessage response = await _httpClient.PostAsync("<LOGIC APP URL>", content);
            }

            if (file is not null)
            {
                var fileName = request.Id + Path.GetExtension(file.FileName);
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("logicappholder");

                var blobClient = containerClient.GetBlobClient(fileName);

                var blobHttpHeader = new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                };

                await blobClient.UploadAsync(file.OpenReadStream(), blobHttpHeader);
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