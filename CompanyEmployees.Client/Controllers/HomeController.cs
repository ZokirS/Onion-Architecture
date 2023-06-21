using CompanyEmployees.Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;
using System.Net;

namespace CompanyEmployees.Client.Controllers
{
    public class HomeController : Controller
    {
        private IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [Authorize]
        public async Task<IActionResult> Companies()
        {
            var httpClient = _httpClientFactory.CreateClient("APIClient");

            var response = await httpClient.GetAsync("api/companies").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var companiesString = await response.Content.ReadAsStringAsync();
                var companies = JsonSerializer.Deserialize<List<CompanyViewModel>>(companiesString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(companies);
            }
            else if(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Auth");
            }
            throw new Exception("There is problem accessing the API.");
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Policy = "CanCreateAndModifyData")]
        public async Task<IActionResult> Privacy()
        {
            var idClient = _httpClientFactory.CreateClient("IDPClient");
            var metaDataResponse = await idClient.GetDiscoveryDocumentAsync();

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var response = await idClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = metaDataResponse.UserInfoEndpoint,
                Token = accessToken
            });
            if (response.IsError)
            {
                throw new Exception("Problem while fetching data UserInfo endpoint", response.Exception);
            }
            var addressClaim = response.Claims.FirstOrDefault(x=>x.Type.Equals("address"));
            User.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(addressClaim.Type.ToString(), addressClaim.Value.ToString())
            }));
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}