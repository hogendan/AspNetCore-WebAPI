

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

    public async Task<List<Walk>> GetAllAsync()
    {
        return await dbContext.Walks.Include("Region").Include("Difficulty").ToListAsync();
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
