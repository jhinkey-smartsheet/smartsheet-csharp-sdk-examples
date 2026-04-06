# Smartsheet C# SDK examples

These .NET programs demonstrate using the Smartsheet C# SDK.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) that supports **.NET 10** (see `TargetFramework` in `GetWorkspaceChildren.csproj`).
- A Smartsheet API access token with access to the workspace.

## Linux: run with workspace ID as an argument

1. Export your API token (session-only; adjust if you use a secrets manager):

   ```bash
   export SMARTSHEET_ACCESS_TOKEN='your-token-here'
   ```

## Example programs

### GetWorkspaceChildren

Console sample that loads a Smartsheet workspace and prints its metadata plus all child sheets, folders, reports, dashboards (sights), and templates (with pagination).

1. From the repository root, go to the project folder and run:

   ```bash
   cd GetWorkspaceChildren
   dotnet run -- 1234567890123456
   ```

   Replace `1234567890123456` with your numeric workspace ID. Everything after `--` is passed to the program as arguments.

Output is JSON written to standard output (workspace first, then each child type).

#### Optional: workspace ID from the environment

If you omit the argument, the program reads `SMARTSHEET_WORKSPACE_ID`:

```bash
export SMARTSHEET_WORKSPACE_ID='1234567890123456'
dotnet run
```
