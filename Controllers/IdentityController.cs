using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Services.AppAuthentication;
using Newtonsoft.Json.Linq;

namespace ManagedServiceIdentityTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        const string RESOURCE = "https://vault.azure.net";
        const string API_VERSION = "2017-09-01";
        static readonly MediaTypeFormatter s_formatter = new JsonMediaTypeFormatter();

        // GET api/values
        [HttpGet("environment")]
        public ActionResult<Dictionary<string, string>> GetEnvironment()
        {
            return new Dictionary<string, string>
            {
                { "Secret", Environment.GetEnvironmentVariable("MSI_SECRET") },
                { "Endpoint", Environment.GetEnvironmentVariable("MSI_ENDPOINT") }
            };
        }

        [HttpGet("token/rest")]
        public async Task<ActionResult> GetTokenRest()
        {
            try
            {
                var secret = Environment.GetEnvironmentVariable("MSI_SECRET");
                var endpoint = Environment.GetEnvironmentVariable("MSI_ENDPOINT");

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Secret", secret);
                var response = await client.GetAsync($"{endpoint}/?resource={RESOURCE}&api-version={API_VERSION}");
                var json = await response.Content.ReadAsAsync<JObject>();
                return Ok(json.Value<string>("access_token"));
            }
            catch(Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpGet("token/appAuth")]
        public async Task<ActionResult> GetTokenAppAuth()
        {
            try 
            {
                var tokenProvider = new AzureServiceTokenProvider();
                var token = await tokenProvider.GetAccessTokenAsync(RESOURCE);

                return Ok(token);
            }
            catch(Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
