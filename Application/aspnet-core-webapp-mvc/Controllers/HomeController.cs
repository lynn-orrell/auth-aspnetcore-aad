using aspnet_core_webapp_mvc.Models;
using aspnet_core_webapp_mvc.Options;
using aspnet_core_webapp_mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace aspnet_core_webapp_mvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly ITokenCacheFactory _tokenCacheFactory;
        private readonly AuthOptions _authOptions;
        private readonly IConfiguration _configuration;

        public HomeController(ITokenCacheFactory tokenCacheFactory, IOptions<AuthOptions> authOptions, IConfiguration configuuration)
        {
            _tokenCacheFactory = tokenCacheFactory;
            _authOptions = authOptions.Value;
            _configuration = configuuration;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Values()
        {
            //var request = new HttpRequestMessage(HttpMethod.Get, "https://robsapimanagement.azure-api.net/api/api/Values");
            var request = new HttpRequestMessage(HttpMethod.Get, _configuration["APIM:Ocp-Apim-Endpoint"]);

            string accessToken = await GetAccessTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _configuration["APIM:Ocp-Apim-Subscription-Key"]);
            request.Headers.Add("Ocp-Apim-Trace", _configuration["Ocp-Apim-Trace"]);

            var response = await Client.SendAsync(request);

            string rawResponse = await response.Content.ReadAsStringAsync();
            string prettyResponse = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(rawResponse), Formatting.Indented);

            var model = new ValuesModel() { PrettyResponse = prettyResponse };

            return View(model);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<string> GetAccessTokenAsync()
        {
            string authority = _authOptions.Authority;

            var cache = _tokenCacheFactory.CreateForUser(User);

            var authContext = new AuthenticationContext(authority, cache);

            //App's credentials may be needed if access tokens need to be refreshed with a refresh token
            string clientId = _authOptions.ClientId;
            string clientSecret = _authOptions.ClientSecret;
            var credential = new ClientCredential(clientId, clientSecret);
            var userId = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");

            var result = await authContext.AcquireTokenSilentAsync(
                _authOptions.Resource,
                credential,
                new UserIdentifier(userId, UserIdentifierType.UniqueId));

            return result.AccessToken;
        }
    }
}
