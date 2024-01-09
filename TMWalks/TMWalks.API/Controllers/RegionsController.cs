using System.Net.Sockets;
using AutoMapper;
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

    public RegionsController(TMWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.regionRepository = regionRepository;
        this.mapper = mapper;
    }

    // GET ALL REGIONS
    // GET: https://localhost:portnumber/api/regions
    [HttpGet]
    public async Task<IActionResult> GetAll() {
        // Get Data From Database - Domain models
        var regionsDomain = await regionRepository.GetAllAsync();

        // Return DTOs
        return Ok(mapper.Map<List<RegionDto>>(regionsDomain));
    }

    // GET SINGLE REGION (Get Region By ID)
    // GET: https://localhost:portnumber/api/regions/{id}
    [HttpGet]
    [Route("{id:Guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id) {
        // Find は Primary Key を探す
        // var region = dbContext.Regions.Find(id);

        // Get Region Domain Model From Database
        var regionDomain = await regionRepository.GetByIdAsync(id);

        if (regionDomain == null) {
            return NotFound();
        }

        // Return DTO back to client
        return Ok(mapper.Map<RegionDto>(regionDomain));
    }

    // POST To Create New Region
    // POST: https://localhost:portnumber/api/regions
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto) {
        if (ModelState.IsValid) {
            // Map or Convert DTO to Domain Model
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

            // Use Domain Model to create Region
            regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

            // Map Domain model back to DTO
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        } else {
            return BadRequest(ModelState);
        }
        
    }

    // Update region
    // PUT: https://localhost:portnumber/api/regions/{id}
    [HttpPut]
    [Route("{id:Guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRetionRequesstDto updateRetionRequesstDto) {
        // Map DTO to Domain Model
        var regionDomainModel = mapper.Map<Region>(updateRetionRequesstDto);

        regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

        if (regionDomainModel == null) {
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
    public async Task<IActionResult> Delete([FromRoute]Guid id) {
        var regionDomainModel = await regionRepository.DeleteAsync(id);

        if (regionDomainModel == null) {
            return NotFound();
        }

        var regionDto = mapper.Map<RegionDto>(regionDomainModel);

        return Ok(regionDto);
    }
}
