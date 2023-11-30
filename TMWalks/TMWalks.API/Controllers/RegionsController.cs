using Microsoft.AspNetCore.Mvc;

namespace TMWalks.API;

[Route("api/[controller]")]
[ApiController]
public class RegionsController : ControllerBase
{
    private readonly TMWalksDbContext dbContext;

    public RegionsController(TMWalksDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // GET ALL REGIONS
    // GET: https://localhost:portnumber/api/regions
    [HttpGet]
    public IActionResult GetAll() {
        var regions = dbContext.Regions.ToList();
        
        return Ok(regions);
    }

    // GET SINGLE REGION (Get Region By ID)
    // GET: https://localhost:portnumber/api/regions/{id}
    [HttpGet]
    [Route("{id:Guid}")]
    public IActionResult GetById([FromRoute] Guid id) {
        // Find は Primary Key を探す
        // var region = dbContext.Regions.Find(id);

        var region = dbContext.Regions.FirstOrDefault(x => x.Id == id);

        if (region == null) {
            return NotFound();
        }

        return Ok(region);
    }
}
