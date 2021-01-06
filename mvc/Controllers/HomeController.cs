using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvc.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> GetData(string type,string code)
        {
            type = type ?? "client";
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
                return new JsonResult(new { err = disco.Error });

            TokenResponse token = null;
            switch (type)
            {
                case "client":
                    token = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
                    {
                        //获取Token的地址
                        Address = disco.TokenEndpoint,
                        //客户端Id
                        ClientId = "apiClientCd",
                        //客户端密码
                        ClientSecret = "apiSecret",
                        //要访问的api资源
                        Scope = "scope1"
                    });
                    break;
                case "password":
                    token = await client.RequestPasswordTokenAsync(new PasswordTokenRequest()
                    { 
                        Address = disco.TokenEndpoint,
                        ClientId = "apiClientPassword",
                        ClientSecret = "apiSecret",
                        Scope = "scope1",
                        UserName = "alice",
                        Password = "alice"
                    });
                    break;
                case "code":
                    var d = new AuthorizationCodeTokenRequest()
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = "apiClientCode",
                        ClientSecret = "apiSecret",
                        Code = code,
                        RedirectUri = "http://localhost:5002/auth_code.html",
                        CodeVerifier = GetCodeVerifier()
                    };

                    token = await client.RequestAuthorizationCodeTokenAsync(d);
                    break;
                default:
                    break;
            }

            
            if (token.IsError)
                return new JsonResult(new { err = token.Error });
            client.SetBearerToken(token.AccessToken);
            string data = await client.GetStringAsync("http://localhost:5001/api/identity");
            JArray json = JArray.Parse(data);
            return new JsonResult(new { res = data });
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Token1()
        {
            var token = await HttpContext.GetTokenAsync("access_token");//OpenIdConnectParameterNames.AccessToken            
            return new JsonResult(token);
        }

        public string GetCodeVerifier()
        {
            return "63-39-E2-08-81-00-7F-5A-E5-CA-F8-91-A0-FF-E3-6E-AE-AB-8D-77";
            RNGCryptoServiceProvider csp = new RNGCryptoServiceProvider();
            byte[] byteCsp = new byte[20];
            csp.GetBytes(byteCsp);
            return BitConverter.ToString(byteCsp);
        }

        public async Task<string> GetCode()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
                return "";

            var obj = new JObject { 
                { "client_id", "apiClientCode" },
                { "response_type", "code" },
                { "scope", "scope1" },
                { "redirect_uri", "http://localhost:5002/auth_code.html" },
                { "code_challenge", GetCodeChallenge(GetCodeVerifier()) },
                { "state", "12345678" }
            };
            var param = @$"?client_id=apiClientCode&redirect_uri=http://localhost:5002/auth_code.html&response_type=code&scope=scope1&code_challenge={GetCodeChallenge(GetCodeVerifier())}&code_challenge_method=S256&state=123";
            HttpContent content = new StringContent(obj.ToString());
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.GetAsync(disco.AuthorizeEndpoint + param);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        [HttpPost]
        public IActionResult GetTokenData(TokenData data)
        {
            return new JsonResult(data);
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Detail()
        {
            var client = new HttpClient();
            var token = await HttpContext.GetTokenAsync("access_token");//OpenIdConnectParameterNames.AccessToken
            client.SetBearerToken(token);// client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string data = await client.GetStringAsync("http://localhost:5001/api/identity");
            JArray json = JArray.Parse(data);
            return new JsonResult(json);
        }

        public string GetCodeChallenge(string codeVerifier)
        {
            var codeVerifierBytes = Encoding.ASCII.GetBytes(codeVerifier);
            var hashedBytes = Sha256(codeVerifierBytes);
            var transformedCodeVerifier = Base64Url.Encode(hashedBytes);
            return transformedCodeVerifier;
        }

        public byte[] Sha256(byte[] input)
        {
            if (input == null)
            {
                return null;
            }

            using var sha = SHA256.Create();
            return sha.ComputeHash(input);
        }
    }

    public class TokenData
    {
        public string code { get; set; }
        public string id_token { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string scope { get; set; }
        public string session_state { get; set; }
    }
}
