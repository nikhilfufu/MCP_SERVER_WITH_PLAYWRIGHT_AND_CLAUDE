using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Microsoft.Playwright;
using System.ComponentModel;
using Reporting;
using AventStack.ExtentReports.Model;
using McpServer;

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

// ============================================================================
// MCP TOOLS - Entry Points
// ============================================================================
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

    [McpServerTool, Description("Test Scenario 1: Valid user login with correct credentials")]
    public static async Task<string> scenario1_ValidLogin(string url, string username, string password)
    {
        var test = ReportManager.CreateTest1("Scenario 1 - Valid Login");
        
        IBrowser? browser = null;
        IPage? page = null;

        try
        {
            test.Info("Starting Playwright");
            using var playwright = await Playwright.CreateAsync();

            test.Info("Launching browser");
            browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions { Headless = false });

            page = await browser.NewPageAsync();
            test.Info($"Navigating to: {url}");
            await page.GotoAsync(url);

            test.Info("Filling username");
            await page.FillAsync("#username", username);

            test.Info("Filling password");
            await page.FillAsync("#password", password);

            test.Info("Clicking submit");
            await page.ClickAsync("#submit");

            test.Info("Waiting for dashboard");
            await page.WaitForSelectorAsync("a[class*='wp-block-button__link has-text-color']",
                new PageWaitForSelectorOptions { Timeout = 10000 });

            test.Pass("Login successful");
            return "Test Passed: Login successful";
        }
        catch (Exception ex)
        {
            test.Fail("Login failed: " + ex.Message);

            string path = null;
            if (page != null)
            {
                path = await ReportManager.TakeScreenshotAsync(page, "ValidLogin_Error");
            }

            if (!string.IsNullOrEmpty(path))
            {
                var media = new ScreenCapture { Path = path };
                ReportManager.FailWithScreenShotAsync(
                    "Login failed in scenario1_ValidLogin",
                    ex.ToString(),
                    media);
            }

            return "Test Failed: " + ex.Message;
        }
        finally
        {
            if (browser != null)
                await browser.CloseAsync();
            ReportManager.FlushAndOpen();
        }
    }

    [McpServerTool, Description("Test Scenario 2: Invalid login with incorrect credentials")]
    public static async Task<string> scenario2_InvalidLogin(string url, string wrongUsername, string wrongPassword)
    {
        var test = ReportManager.CreateTest1("Scenario 2 - Invalid Login");
        
        IBrowser? browser = null;
        IPage? page = null;

        try
        {
            test.Info("Starting Playwright");
            using var playwright = await Playwright.CreateAsync();

            test.Info("Launching browser");
            browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions { Headless = false });

            page = await browser.NewPageAsync();
            test.Info($"Navigating to: {url}");
            await page.GotoAsync(url);

            test.Info("Filling invalid username");
            await page.FillAsync("#username", wrongUsername);

            test.Info("Filling invalid password");
            await page.FillAsync("#password", wrongPassword);

            test.Info("Clicking submit");
            await page.ClickAsync("#submit");

            test.Info("Checking for error message");
            await Task.Delay(3000);
            
            var currentUrl = page.Url;
            if (await page.Locator("#error").IsVisibleAsync())
            {
                test.Pass("Invalid login correctly failed");
                return "Test Passed: Invalid login correctly failed";
            }
            else
            {
                test.Fail("Invalid login was accepted (security issue!)");
                return "Test Failed: Invalid credentials were accepted";
            }
        }
        catch (Exception ex)
        {
            test.Info("Exception caught (expected for invalid login): " + ex.Message);
            test.Pass("Invalid login correctly failed with error");
            return "Test Passed: Invalid login failed as expected";
        }
        finally
        {
            if (browser != null)
                await browser.CloseAsync();
            ReportManager.FlushAndOpen();
        }
    }

    [McpServerTool, Description("Test Scenario 3: Login attempt with empty username/password fields")]
    public static async Task<string> scenario3_EmptyFields(string url)
    {
        var test = ReportManager.CreateTest1("Scenario 3 - Empty Fields Validation");
        
        IBrowser? browser = null;
        IPage? page = null;

        try
        {
            test.Info("Starting Playwright");
            using var playwright = await Playwright.CreateAsync();

            test.Info("Launching browser");
            browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions { Headless = false });

            page = await browser.NewPageAsync();
            test.Info($"Navigating to: {url}");
            await page.GotoAsync(url);

            test.Info("Attempting to click submit without filling fields");
            await page.ClickAsync("#submit");

            await Task.Delay(2000);
            
            var currentUrl = page.Url;
            if (await page.Locator("#error").IsVisibleAsync())
            {
                test.Pass("Empty fields validation working correctly");
                return "Test Passed: Empty fields were validated";
            }
            else
            {
                test.Fail("Login succeeded with empty fields (validation issue!)");
                
                string path = null;
                if (page != null)
                {
                    path = await ReportManager.TakeScreenshotAsync(page, "EmptyFields_Error");
                }

                if (!string.IsNullOrEmpty(path))
                {
                    var media = new ScreenCapture { Path = path };
                    ReportManager.FailWithScreenShotAsync(
                        "Empty fields validation failed",
                        "Login succeeded with empty credentials",
                        media);
                }
                
                return "Test Failed: Empty fields were not validated";
            }
        }
        catch (Exception ex)
        {
            test.Fail("Unexpected error: " + ex.Message);
            return "Test Error: " + ex.Message;
        }
        finally
        {
            if (browser != null)
                await browser.CloseAsync();
            ReportManager.FlushAndOpen();
        }
    }

    [McpServerTool, Description("Run all three test scenarios sequentially with detailed pass/fail analysis")]
    public static async Task<string> runAllScenarios(
        string url, 
        string validUsername, 
        string validPassword,
        string invalidUsername = "wronguser",
        string invalidPassword = "wrongpass")
    {
        var results = new List<TestResult>();
        int passed = 0, failed = 0;
        
        ReportManager.CreateTest1("Test Suite - All Scenarios");

        // Run Scenario 1
        var result1 = await ExecuteScenarioWithDetails(
            "Scenario 1: Valid Login Test",
            async () => await scenario1_ValidLogin(url, validUsername, validPassword),
            "Tests that a valid user can successfully log in with correct credentials"
        );
        results.Add(result1);
        if (result1.Passed) passed++; else failed++;
        await Task.Delay(2000);

        // Run Scenario 2
        var result2 = await ExecuteScenarioWithDetails(
            "Scenario 2: Invalid Login Test",
            async () => await scenario2_InvalidLogin(url, invalidUsername, invalidPassword),
            "Tests that invalid credentials are properly rejected by the system"
        );
        results.Add(result2);
        if (result2.Passed) passed++; else failed++;
        await Task.Delay(2000);

        // Run Scenario 3
        var result3 = await ExecuteScenarioWithDetails(
            "Scenario 3: Empty Fields Validation",
            async () => await scenario3_EmptyFields(url),
            "Tests that the login form validates empty username/password fields"
        );
        results.Add(result3);
        if (result3.Passed) passed++; else failed++;

        return GPTAnalysis.FormatDetailedReport(results, passed, failed);
    }

    private static async Task<TestResult> ExecuteScenarioWithDetails(
        string scenarioName,
        Func<Task<string>> testAction,
        string description)
    {
        var result = new TestResult
        {
            ScenarioName = scenarioName,
            Description = description,
            StartTime = DateTime.Now
        };

        try
        {
            var output = await testAction();
            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;
            result.Output = output;
            result.Passed = output.Contains("Test Passed");
            
            if (result.Passed)
            {
                result.Reason = "All validations passed successfully";
            }
            else
            {
                result.Reason = FailureAnalyzier.ExtractFailureReason(output);
            }
        }
        catch (Exception ex)
        {
            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;
            result.Passed = false;
            result.Reason = $"Exception occurred: {ex.Message}";
            result.Output = ex.ToString();
        }

        return result;
    }

  public class TestResult
    {
        public string ScenarioName { get; set; }
        public string Description { get; set; }
        public bool Passed { get; set; }
        public string Reason { get; set; }
        public string Output { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
    }

}