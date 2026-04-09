using Newtonsoft.Json;
using Smartsheet.Api;
using Smartsheet.Api.Models;

const string tokenEnv = "SMARTSHEET_ACCESS_TOKEN";
const string workspaceEnv = "SMARTSHEET_WORKSPACE_ID";

var token = Environment.GetEnvironmentVariable(tokenEnv);
if (string.IsNullOrWhiteSpace(token))
{
    Console.Error.WriteLine($"Set {tokenEnv} to your Smartsheet API access token.");
    Environment.Exit(1);
}

var workspaceArg = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable(workspaceEnv);
if (string.IsNullOrWhiteSpace(workspaceArg))
{
    Console.Error.WriteLine("Usage: GetWorkspaceChildren <workspaceId>");
    Console.Error.WriteLine($"Or set {workspaceEnv} and run with no arguments.");
    Environment.Exit(1);
}

if (!long.TryParse(workspaceArg, out var workspaceId))
{
    Console.Error.WriteLine("Workspace id must be a numeric id.");
    Environment.Exit(1);
}

SmartsheetClient client = new SmartsheetBuilder()
    .SetAccessToken(token)
    .Build();

Workspace workspace =
    client.WorkspaceResources.GetWorkspaceMetadata(workspaceId);

Console.WriteLine($"Workspace\n " + 
    $"name: {workspace.Name}\n " +
    $"id: {workspace.Id}\n " +
    $"access level: {workspace.AccessLevel}\n " +
    $"permalink: {workspace.Permalink}\n " +
    $"created at: {workspace.CreatedAt}\n " +
    $"modified at: {workspace.ModifiedAt}\n ");

List<Sheet> sheets = new();
List<Folder> folders = new();
List<Report> reports = new();
List<Sight> sights = new();
List<Template> templates = new();

string? lastKey = null;
do
{
    TokenPaginatedResult<object> page = client.WorkspaceResources.GetWorkspaceChildren(
        workspaceId,
        childrenResourceTypes: null,
        include: null,
        numericDates: null,
        accessApiLevel: null,
        lastKey: lastKey,
        maxItems: null);

    foreach (var item in page.Data)
    {
        switch (item)
        {
            case Sheet sheet:
                sheets.Add(sheet);
                break;
            case Folder folder:
                folders.Add(folder);
                break;
            case Report report:
                reports.Add(report);
                break;
            case Sight sight:
                sights.Add(sight);
                break;
            case Template template:
                templates.Add(template);
                break;
        }

    }

    lastKey = page.LastKey;
} while (!string.IsNullOrEmpty(lastKey));

Console.WriteLine("=== Workspace ===");
Console.WriteLine(JsonConvert.SerializeObject(workspace, Formatting.Indented));
Console.WriteLine();

WriteJsonList("Sheets", sheets);
WriteJsonList("Folders", folders);
WriteJsonList("Reports", reports);
WriteJsonList("Sights", sights);
WriteJsonList("Templates", templates);

void WriteJsonList<T>(string label, List<T> items)
{
    Console.WriteLine($"=== {label} ({items.Count}) ===");
    foreach (var item in items)
    {
        Console.WriteLine(JsonConvert.SerializeObject(item, Formatting.Indented));
        Console.WriteLine();
    }
}
