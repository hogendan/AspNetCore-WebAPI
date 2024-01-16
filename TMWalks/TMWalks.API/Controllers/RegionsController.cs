using System.Net.Sockets;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using TMWalks.API.Repositories;

namespace TMWalks.API;

[Route("api/[controller]")]
[ApiController]
public class RegionsController : ControllerBase
{
    private readonly TMWalksDbContext dbContext;
    private readonly IRegionRepository regionRepository;
    private readonly IMapper mapper;
    private readonly ILogger<RegionsController> logger;

    public RegionsController(TMWalksDbContext dbContext,
                             IRegionRepository regionRepository,
                             IMapper mapper,
                             ILogger<RegionsController> logger)
    {
        this.dbContext = dbContext;
        this.regionRepository = regionRepository;
        this.mapper = mapper;
        this.logger = logger;
    }

    // GET ALL REGIONS
    // GET: https://localhost:portnumber/api/regions
    [HttpGet]
    // [Authorize(Roles = "Reader, Writer")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            logger.LogInformation("GetAllRegions Action Method was invoked");

            // Get Data From Database - Domain models
            var regionsDomain = await regionRepository.GetAllAsync();

            logger.LogInformation($"Finished GetAllRegions request with data: {JsonSerializer.Serialize(regionsDomain)}");

            // Return DTOs
            return Ok(mapper.Map<List<RegionDto>>(regionsDomain));
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw;
        }
    }

    // GET SINGLE REGION (Get Region By ID)
    // GET: https://localhost:portnumber/api/regions/{id}
    [HttpGet]
    [Route("{id:Guid}")]
    [Authorize(Roles = "Reader, Writer")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        // Find は Primary Key を探す
        // var region = dbContext.Regions.Find(id);

        // Get Region Domain Model From Database
        var regionDomain = await regionRepository.GetByIdAsync(id);

        if (regionDomain == null)
        {
            return NotFound();
        }

        // Return DTO back to client
        return Ok(mapper.Map<RegionDto>(regionDomain));
    }

    // POST To Create New Region
    // POST: https://localhost:portnumber/api/regions
    [HttpPost]
    [ValidateModel]
    // [Authorize(Roles = "Writer")]
    public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
    {
        // Map or Convert DTO to Domain Model
        var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

        // Use Domain Model to create Region
        regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

        // Map Domain model back to DTO
        var regionDto = mapper.Map<RegionDto>(regionDomainModel);

        return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
    }

    // Update region
    // PUT: https://localhost:portnumber/api/regions/{id}
    [HttpPut]
    [Route("{id:Guid}")]
    [ValidateModel]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRetionRequesstDto updateRetionRequesstDto)
    {
        // Map DTO to Domain Model
        var regionDomainModel = mapper.Map<Region>(updateRetionRequesstDto);

        regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

        if (regionDomainModel == null)
        {
            return NotFound();
        }

        // Convert Domain Model to DTO
        var regionDto = mapper.Map<RegionDto>(regionDomainModel);

        return Ok(regionDto);
    }

    // Delete region
    // DELETE: https://localhost:portnumber/api/region/{id}
    [HttpDelete]
    [Route("{id:Guid}")]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var regionDomainModel = await regionRepository.DeleteAsync(id);

        if (regionDomainModel == null)
        {
            return NotFound();
        }

        var regionDto = mapper.Map<RegionDto>(regionDomainModel);

        return Ok(regionDto);
    }
}
