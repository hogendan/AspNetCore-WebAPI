using Microsoft.AspNetCore.Mvc;

namespace TMWalks.API;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[ApiController]
public class CountryController : ControllerBase
{
    [MapToApiVersion("1.0")]
    [HttpGet]
    public IActionResult GetV1() 
    {
        var dto = new CountryDtoV1 {
            Id = 1,
            Name = "Japan v1",
        };

        return Ok(dto);
    }

    [MapToApiVersion("2.0")]
    [HttpGet]
    public IActionResult GetV2() 
    {
        var dto = new CountryDtoV2 {
            Id = 1,
            CountryName = "Japan v2",
        };

        return Ok(dto);
    }
}
