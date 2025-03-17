using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/dynamics")]
public class DynamicsController : ControllerBase
{
    private const string InstanceUrl = "https://org7abba8db.crm8.dynamics.com/";
    private const string ApiUrl = "/api/data/v9.2/gv_bank_loans(137d1884-47d7-ef11-8eea-7c1e523d27d3)";
    private const string ActionApiUrl = "/api/data/v9.2/gv_bank_accounts(280af31b-b394-49b8-befe-28bf84ca5a8a)/Microsoft.Dynamics.CRM.gv_action_calculateTotalLoanAmt"; // action
    private const string ClientId = "f63fd72e-6ff0-4721-b32d-eed68731e232";
    private const string ClientSCValue = "kBR8Q~CHTbpM_F.ozS6vfZnmyPrM_r~Lcti5Acax";
    private const string TenantId = "f976b35f-fc26-4ef7-a853-ef1ab274a897";

    [HttpGet("TestAPI")]
    public async Task<IActionResult> TestAPI()
    {
        return Ok("API is up and running");
    }

    // https://localhost:5001/api/dynamics/call
    [HttpGet("call")]
    public async Task<IActionResult> CallDynamicsApi()
    {
        string token = await GetAccessToken();
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(InstanceUrl+ApiUrl);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        return Ok(await response.Content.ReadAsStringAsync());
    }

    //https://localhost:5001/api/dynamics/callaction
    [HttpPost("callaction")]
    public async Task<IActionResult> CallActionDynamicsApi([FromBody] ActionRequest request)
    {
        string token = await GetAccessToken();
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var payload = new { accountMinBalance = request.accountMinBalance };
        var jsonPayload = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(InstanceUrl + ActionApiUrl, jsonPayload);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        return Ok(await response.Content.ReadAsStringAsync());
    }

    private async Task<string> GetAccessToken()
    {
        var app = ConfidentialClientApplicationBuilder
            .Create(ClientId)
            .WithClientSecret(ClientSCValue)
            .WithAuthority($"https://login.microsoftonline.com/{TenantId}")
            .Build();

        string[] scopes = { "https://org7abba8db.crm8.dynamics.com/.default" };

        var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
        return result.AccessToken;
    }

    public class ActionRequest
    {
        public string accountMinBalance { get; set; }
    }
}