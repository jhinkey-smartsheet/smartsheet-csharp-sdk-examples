using Newtonsoft.Json;
using Smartsheet.Api;
using Smartsheet.Api.Models;

const string tokenEnv = "SMARTSHEET_API_TOKEN";

var token = Environment.GetEnvironmentVariable(tokenEnv);
if (string.IsNullOrWhiteSpace(token))
{
    Console.Error.WriteLine($"Set {tokenEnv} to your Smartsheet API access token.");
    Environment.Exit(1);
}

if (args.Length < 1 || string.IsNullOrWhiteSpace(args[0]))
{
    Console.Error.WriteLine("Usage: LegacyGetWorkspace <workspaceId>");
    Environment.Exit(1);
}

if (!long.TryParse(args[0], out var workspaceId))
{
    Console.Error.WriteLine("Workspace id must be a numeric id.");
    Environment.Exit(1);
}

SmartsheetClient client = new SmartsheetBuilder()
    .SetAccessToken(token)
    .Build();

#pragma warning disable CS0618 // Intentional: legacy GetWorkspace API (deprecated in favor of GetWorkspaceChildren + GetWorkspaceMetadata).
Workspace workspace = client.WorkspaceResources.GetWorkspace(workspaceId, null, null);
#pragma warning restore CS0618

var folders = workspace.Folders;
var sheets = workspace.Sheets;
var reports = workspace.Reports;
var sights = workspace.Sights;
var templates = workspace.Templates;

Console.WriteLine("=== Workspace ===");
Console.WriteLine(JsonConvert.SerializeObject(workspace, Formatting.Indented));
Console.WriteLine();

WriteJsonList("Folders", folders);
WriteJsonList("Sheets", sheets);
WriteJsonList("Sights", sights);
WriteJsonList("Reports", reports);
WriteJsonList("Templates", templates);

void WriteJsonList<T>(string label, IEnumerable<T>? items)
{
    var list = items?.ToList() ?? new List<T>();
    Console.WriteLine($"=== {label} ({list.Count}) ===");
    foreach (var item in list)
    {
        Console.WriteLine(JsonConvert.SerializeObject(item, Formatting.Indented));
        Console.WriteLine();
    }
}
