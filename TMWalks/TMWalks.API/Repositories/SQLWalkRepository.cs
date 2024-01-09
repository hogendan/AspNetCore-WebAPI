

using Microsoft.EntityFrameworkCore;

namespace TMWalks.API;

public class SQLWalkRepository : IWalkRepository
{
    private readonly TMWalksDbContext dbContext;

    public SQLWalkRepository(TMWalksDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<Walk> CreateAsync(Walk walk)
    {
        await dbContext.Walks.AddAsync(walk);
        await dbContext.SaveChangesAsync();
        return walk;
    }

    public async Task<Walk?> DeleteAsync(Guid id)
    {
        var walk = await dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);
        if (walk == null) return null;

        dbContext.Walks.Remove(walk);
        await dbContext.SaveChangesAsync();

        return walk;
    }

    public async Task<List<Walk>> GetAllAsync(string? filterOn = null,
                                              string? filterQuery = null,
                                              string? sortBy = null,
                                              bool isAcsending = true,
                                              int pageNumber = 1,
                                              int pageSize = 1000)
    {
        var walks = dbContext.Walks.Include("Region").Include("Difficulty").AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
        {
            if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                walks = walks.Where(x => x.Name.Contains(filterQuery));
            }

        }

        // Sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                walks = isAcsending ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
            }
            else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
            {
                walks = isAcsending ? walks.OrderBy(x => x.LengthInKmj) : walks.OrderByDescending(x => x.LengthInKmj);
            }
        }

        // Pagination
        var skipResults = (pageNumber - 1) * pageSize;

        return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
    }

    public async Task<Walk?> GetByIdAsync(Guid id)
    {
        return await dbContext.Walks
            .Include("Region")
            .Include("Difficulty")
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
    {
        var target = await dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);
        if (target == null) return null;

        target.Name = walk.Name;
        target.LengthInKmj = walk.LengthInKmj;
        target.WalkImageUrl = walk.WalkImageUrl;
        target.Description = walk.Description;
        target.DifficultyId = walk.DifficultyId;
        target.RegionId = walk.RegionId;

        await dbContext.SaveChangesAsync();

        return target;
    }
}
