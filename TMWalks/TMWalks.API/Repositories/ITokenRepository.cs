using Microsoft.AspNetCore.Identity;

namespace TMWalks.API;

public interface ITokenRepository
{
    string CreateJWTToken(IdentityUser user, List<string> roles);
}
