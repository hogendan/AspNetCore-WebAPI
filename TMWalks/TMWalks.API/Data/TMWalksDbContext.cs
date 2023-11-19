using Microsoft.EntityFrameworkCore;

namespace TMWalks.API;

public class TMWalksDbContext : DbContext
{
    public TMWalksDbContext(DbContextOptions dbContextOptions): base(dbContextOptions)
    {
        
    }

    public DbSet<Difficulty> Difficulties { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Walk> Walks { get; set; }
}
