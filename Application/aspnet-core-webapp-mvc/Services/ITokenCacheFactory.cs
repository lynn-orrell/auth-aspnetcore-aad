using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Security.Claims;

namespace aspnet_core_webapp_mvc.Services
{
    public interface ITokenCacheFactory
    {
        TokenCache CreateForUser(ClaimsPrincipal user);
    }
}
