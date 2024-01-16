using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TMWalker.UI.Models;

namespace TMWalker.UI;

public class RegionsController : Controller
{
    private readonly IHttpClientFactory httpClientFactory;

    public RegionsController(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<RegionDto> response = new();

        try
        {
            // Get All Regions from Web API
            var client = httpClientFactory.CreateClient();

            var httpResponseMessage = await client.GetAsync("https://localhost:7017/api/regions");

            // HTTP response が false の時に例外発生する
            httpResponseMessage.EnsureSuccessStatusCode();

            response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());
        }
        catch (System.Exception)
        {
            // Log the exceptions
        }

        return View(response);
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Add(AddRegionViewModel model)
    {
        var client = httpClientFactory.CreateClient();

        var httpRequestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://localhost:7017/api/regions"),
            Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"),
        };

        var httpResponseMessage = await client.SendAsync(httpRequestMessage);

try
{
            httpResponseMessage.EnsureSuccessStatusCode();
    
}
catch (System.Exception e)
{
    var hoge = await httpResponseMessage.Content.ReadAsStringAsync();
    Console.WriteLine(e);
    throw;
}
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

        if (response is not null)
        {
            return RedirectToAction("Index", "Regions");
        }

        return View();
    }
}
