using Newtonsoft.Json;
using Smartsheet.Api;
using Smartsheet.Api.Models;

const string tokenEnv = "SMARTSHEET_ACCESS_TOKEN";
const string folderEnv = "FOLDER_ID";

var token = Environment.GetEnvironmentVariable(tokenEnv);
if (string.IsNullOrWhiteSpace(token))
{
    Console.Error.WriteLine($"Set {tokenEnv} to your Smartsheet API access token.");
    Environment.Exit(1);
}

var folderArg = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable(folderEnv);
if (string.IsNullOrWhiteSpace(folderArg))
{
    Console.Error.WriteLine("Usage: GetFolderChildren <folderId>");
    Console.Error.WriteLine($"Or set {folderEnv} and run with no arguments.");
    Environment.Exit(1);
}

if (!long.TryParse(folderArg, out var folderId))
{
    Console.Error.WriteLine("Folder id must be a numeric id.");
    Environment.Exit(1);
}

SmartsheetClient client = new SmartsheetBuilder()
    .SetAccessToken(token)
    .Build();

Folder folderMetadata =
    client.FolderResources.GetFolderMetadata(folderId);

Console.WriteLine($"Parent Folder\n " + 
    $"name: {folderMetadata.Name}\n " +
    $"id: {folderMetadata.Id}\n " +
    $"permalink: {folderMetadata.Permalink}\n " +
    $"created at: {folderMetadata.CreatedAt}\n " +
    $"modified at: {folderMetadata.ModifiedAt}\n ");

List<Sheet> sheets = new();
List<Folder> folders = new();
List<Report> reports = new();
List<Sight> sights = new();
List<Template> templates = new();

string? lastKey = null;
do
{
    TokenPaginatedResult<object> page =
        client.FolderResources.GetFolderChildren(
            folderId,
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

Console.WriteLine("=== Folder ===");
Console.WriteLine(JsonConvert.SerializeObject(folderMetadata, Formatting.Indented));
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
