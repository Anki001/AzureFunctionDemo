using AzureFunctionWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class GroceryController : Controller
{
    readonly HttpClient _httpClient;
    readonly string _groceryAPIUrl;

    public GroceryController(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _groceryAPIUrl = configuration.GetValue<string>("GroceryAPIUrl");
    }

    // GET: GroceryController
    public async Task<ActionResult> Index()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(_groceryAPIUrl);
        string returnValue = await response.Content.ReadAsStringAsync();
        List<GroceryItem> groceryListToReturn = JsonConvert.DeserializeObject<List<GroceryItem>>(returnValue);
        return View(groceryListToReturn);
    }

    // GET: GroceryController/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: GroceryController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(GroceryItem obj)
    {
        try
        {
            var jsonContent = JsonConvert.SerializeObject(obj);
            using (var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_groceryAPIUrl, content);
                string returnValue = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // GET: GroceryController/Edit/5
    public async Task<ActionResult> Edit(string id)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(_groceryAPIUrl + "/" + id);
        string returnValue = response.Content.ReadAsStringAsync().Result;
        GroceryItem groceryItem = JsonConvert.DeserializeObject<GroceryItem>(returnValue);
        return View(groceryItem);
    }

    // POST: GroceryController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(GroceryItem obj)
    {
        try
        {
            var jsonContent = JsonConvert.SerializeObject(obj);
            using (var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage response = await _httpClient.PutAsync(_groceryAPIUrl + "/" + obj.Id, content);
                string returnValue = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // GET: GroceryController/Delete/5
    public async Task<ActionResult> Delete(string id)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(_groceryAPIUrl + "/" + id);
        string returnValue = response.Content.ReadAsStringAsync().Result;
        GroceryItem groceryItem = JsonConvert.DeserializeObject<GroceryItem>(returnValue);
        return View(groceryItem);
    }

    // POST: GroceryController/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeletePOST(string id)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(_groceryAPIUrl + "/" + id);
            string returnValue = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}