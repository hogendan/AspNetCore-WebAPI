using Microsoft.AspNetCore.Mvc;

namespace TMWalker.UI;

public class RegionsController : Controller
{
    private readonly IHttpClientFactory httpClientFactory;

    public RegionsController(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        List<RegionDto> response = new ();

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
}
