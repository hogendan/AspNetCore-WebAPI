using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TMWalks.API;

public class TMWalksAuthDbContext : IdentityDbContext
{
    public TMWalksAuthDbContext(DbContextOptions<TMWalksAuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var readerRoleId = "4a947f13-a49d-4882-bec9-e6c700ddf326";
        var writerRoleId = "4420a419-4ac4-4bde-a0e5-97dc4153812f";

        var roles = new List<IdentityRole> {
            new IdentityRole {
                Id = readerRoleId,
                ConcurrencyStamp = readerRoleId,
                Name = "Reader",
                NormalizedName = "Reader".ToUpper()
            },
            new IdentityRole {
                Id = writerRoleId,
                ConcurrencyStamp = writerRoleId,
                Name = "writer",
                NormalizedName = "writer".ToUpper()
            },
        };

        builder.Entity<IdentityRole>().HasData(roles);
    }
}
