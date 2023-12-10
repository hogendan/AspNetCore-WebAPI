using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMWalks.API.Repositories;

namespace TMWalks.API;

[Route("api/[controller]")]
[ApiController]
public class RegionsController : ControllerBase
{
    private readonly TMWalksDbContext dbContext;
    private readonly IRegionRepository regionRepository;

    public RegionsController(TMWalksDbContext dbContext, IRegionRepository regionRepository)
    {
        this.dbContext = dbContext;
        this.regionRepository = regionRepository;
    }

    // GET ALL REGIONS
    // GET: https://localhost:portnumber/api/regions
    [HttpGet]
    public async Task<IActionResult> GetAll() {
        // Get Data From Database - Domain models
        var regionsDomain = await regionRepository.GetAllAsync();
        
        // Map Domain Models to DTOs
         var regionsDto = new List<RegionDto>();
         foreach(var regionDomain in regionsDomain) {
            regionsDto.Add(new RegionDto {
                Id = regionDomain.Id,
                Code = regionDomain.Code,
                Name = regionDomain.Name,
                RegionImageUrl = regionDomain.RegionImageUrl,
            });
         }

        // Return DTOs
        return Ok(regionsDto);
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

        // Map/Converter Region Domain Model to Region DTO
        var regionDto = new RegionDto {
                Id = regionDomain.Id,
                Code = regionDomain.Code,
                Name = regionDomain.Name,
                RegionImageUrl = regionDomain.RegionImageUrl,
        };

        // Return DTO back to client
        return Ok(regionDto);
    }

    // POST To Create New Region
    // POST: https://localhost:portnumber/api/regions
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto) {
        // Map or Convert DTO to Domain Model
        var regionDomainModel = new Region {
            Code = addRegionRequestDto.Code,
            Name = addRegionRequestDto.Name,
            RegionImageUrl = addRegionRequestDto.RegionImageUrl,
        };

        // Use Domain Model to create Region
        regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

        // Map Domain model back to DTO
        var regionDto = new RegionDto {
            Id = regionDomainModel.Id,
            Code = regionDomainModel.Code,
            Name = regionDomainModel.Name,
            RegionImageUrl = regionDomainModel.RegionImageUrl,
        };

        return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
    }

    // Update region
    // PUT: https://localhost:portnumber/api/regions/{id}
    [HttpPut]
    [Route("{id:Guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRetionRequesstDto updateRetionRequesstDto) {
        // Map DTO to Domain Model
        var regionDomainModel = new Region {
            Code = updateRetionRequesstDto.Code,
            Name = updateRetionRequesstDto.Name,
            RegionImageUrl = updateRetionRequesstDto.RegionImageUrl,
        };

        regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

        if (regionDomainModel == null) {
            return NotFound();
        }

        // Convert Domain Model to DTO
        var regionDto = new RegionDto {
            Id = regionDomainModel.Id,
            Code = regionDomainModel.Code,
            Name = regionDomainModel.Name,
            RegionImageUrl = regionDomainModel.RegionImageUrl,
        };

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

        var regionDto = new RegionDto {
            Id = regionDomainModel.Id,
            Code = regionDomainModel.Code,
            Name = regionDomainModel.Name,
            RegionImageUrl = regionDomainModel.RegionImageUrl,
        };

        return Ok(regionDto);
    }
}
