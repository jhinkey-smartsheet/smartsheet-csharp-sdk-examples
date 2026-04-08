using Newtonsoft.Json;
using Smartsheet.Api;
using Smartsheet.Api.Models;

const string tokenEnv = "SMARTSHEET_ACCESS_TOKEN";

var token = Environment.GetEnvironmentVariable(tokenEnv);
if (string.IsNullOrWhiteSpace(token))
{
    Console.Error.WriteLine($"Set {tokenEnv} to your Smartsheet API access token.");
    Environment.Exit(1);
}

SmartsheetClient client = new SmartsheetBuilder()
    .SetAccessToken(token)
    .Build();

string? lastKey = null;
do
{
    TokenPaginatedResult<Workspace> page = client.WorkspaceResources.ListWorkspaces(
        new ListWorkspacesTokenPaginationParameters(lastKey, 100, "token"));

    foreach (var item in page.Data)
    {
        Console.WriteLine(JsonConvert.SerializeObject(item, Formatting.Indented));
    }

    lastKey = page.LastKey;
} while (!string.IsNullOrEmpty(lastKey));
