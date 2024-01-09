using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TMWalks.API;

public class TMWalksAuthDbContext : IdentityDbContext
{
    public TMWalksAuthDbContext(DbContextOptions<TMWalksAuthDbContext> options) : base(options)
    {
    }
}
