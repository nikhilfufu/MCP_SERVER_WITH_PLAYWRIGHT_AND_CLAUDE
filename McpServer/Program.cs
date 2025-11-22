using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Microsoft.Playwright;
using System.ComponentModel;
using Reporting;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(o =>
{
    o.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();

[McpServerToolType]
public class McpTools 
{
    [McpServerTool, Description("Says hello to the given name.")]
    public static string sayHello(string name)
        => $"Hello {name} from C# MCP!";

    [McpServerTool, Description("Returns all registered MCP tools.")]
    public static string list_tools()
    {
        var methods = typeof(McpTools).Assembly
            .GetTypes()
            .SelectMany(t => t.GetMethods(
                System.Reflection.BindingFlags.Public
              | System.Reflection.BindingFlags.NonPublic
              | System.Reflection.BindingFlags.Static
              | System.Reflection.BindingFlags.Instance))
            .Where(m => m.GetCustomAttributes(false)
                .Any(a => a.GetType().Name == "McpServerToolAttribute"))
            .Select(m => m.Name)
            .Distinct()
            .ToList();

        return methods.Count > 0
            ? string.Join("\n", methods)
            : "No tools found.";
    }

    [McpServerTool, Description("Opens a page and attempts login with given credentials.")]
    public static async Task<string> browserLogin(string url, string username, string password)
    {
        var test = ReportManager.CreateTest1($"browser_login - {url}");

        IBrowser? browser = null;
        IPage? page = null;

        try
        {
            test.Info("Starting Playwright");
            using var playwright = await Playwright.CreateAsync();

            test.Info("Launching browser (headed)");
            browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions { Headless = false });

            page = await browser.NewPageAsync();
            test.Info($"Navigating to: {url}");
            await page.GotoAsync(url);

            test.Info("Filling username");
            await page.FillAsync("#txtUsername", username);

            test.Info("Filling password");
            await page.FillAsync("#txtPassword", password);

            test.Info("Clicking submit");
            await page.ClickAsync("#btnLogin");

            test.Info("Waiting for dashboard selector");
            await page.WaitForSelectorAsync("#grid-Gather",
                new PageWaitForSelectorOptions { Timeout = 5000 });

            test.Pass("Login successful");
            return "Login successful";
        }
        catch (Exception ex)
        {
            test.Fail("Login failed: " + ex.Message);

            try
            {
                if (page != null)
                {
                    var screenshotsDir = Path.Combine(AppContext.BaseDirectory, "Screenshots");
                    Directory.CreateDirectory(screenshotsDir);

                    var fileName = $"browser_login_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    var fullPath = Path.Combine(screenshotsDir, fileName);

                    await page.ScreenshotAsync(new PageScreenshotOptions
                    {
                        Path = fullPath,
                        FullPage = true
                    });

                    test.AddScreenCaptureFromPath(fullPath);
                }
            }
            catch
            {
                // ignore screenshot failure
            }

            throw;
        }
        finally
        {
            if (browser != null)
                await browser.CloseAsync();

            ReportManager.FlushAndOpen();
        }
    }
}