using System;
using System.Diagnostics;
using System.IO;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Model;
using AventStack.ExtentReports.Reporter;
using Microsoft.Playwright;

namespace Reporting
{
    public static class ReportManager
    {
        // Global Extent + current test
        public static ExtentReports _extent;
        public static ExtentTest extentTest;

        public static void SetCurrentTest(ExtentTest test) => extentTest = test;
        public static ExtentTest GetCurrentTest => extentTest;

        private static readonly Lazy<ExtentReports> _lazyExtent = new(() =>
        {
            var reportsDir = Path.Combine(AppContext.BaseDirectory, "Reports");
            Directory.CreateDirectory(reportsDir);

            var reportPath = Path.Combine(
                reportsDir,
                $"ExtentReport_{DateTime.Now:yyyyMMdd_HHmmss}.html"
            );

            var spark = new ExtentSparkReporter(reportPath);
            spark.Config.DocumentTitle = "MCP Automation Report";
            spark.Config.ReportName = "MCP Automation Execution";

            var extent = new ExtentReports();
            extent.AttachReporter(spark);

            extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
            extent.AddSystemInfo("Machine", Environment.MachineName);
            extent.AddSystemInfo(".NET", Environment.Version.ToString());

            _reportPath = reportPath;
            _extent = extent;

            return extent;
        });

        private static string? _reportPath;
        public static ExtentReports Extent => _lazyExtent.Value;

        // Create test and remember it as "current"
        public static ExtentTest CreateTest1(string testName)
        {
            var test = Extent.CreateTest(testName);
            SetCurrentTest(test);             // IMPORTANT: this fixes the null
            return test;
        }

        /// <summary>
        /// Log failure to current test and optionally attach screenshot.
        /// </summary>
        public static void FailWithScreenShotAsync(
            string message = null,
            string errorTrace = null,
            AventStack.ExtentReports.Model.Media media = null)
        {
            var currentTest = GetCurrentTest;
            if (currentTest == null)
            {
                // No active test – don't crash, just return
                return;
            }

            var errormsg = $"Error: {message ?? string.Empty}";

            if (!string.IsNullOrWhiteSpace(errorTrace))
            {
                errormsg += Environment.NewLine + errorTrace;
            }

            // If media is null, Extent will just log without screenshot
            currentTest.Log(Status.Fail, errormsg, media);
        }

        /// <summary>
        /// Flush report and try to open it in the default browser (optional).
        /// Call this from your test if you want auto-open.
        /// </summary>
        public static void FlushAndOpen()
        {
            Extent.Flush();

            try
            {
                if (!string.IsNullOrWhiteSpace(_reportPath) && File.Exists(_reportPath))
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = _reportPath,
                        UseShellExecute = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                    Process.Start(psi);
                }
            }
            catch
            {
                // Ignore – report is still written on disk
            }
        }
    public static async Task<string> TakeScreenshotAsync(IPage page, string fileName)
        {
            var directoryPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            var invalidChars = Path.GetInvalidFileNameChars();
            var safeFileName = new string(fileName.Select(ch =>
                invalidChars.Contains(ch) ? '_' : ch).ToArray());

            string fileNameBase = $"Error_{safeFileName.Substring(0, Math.Min(50, safeFileName.Length))}_{timestamp}";
            var path = Path.Combine(directoryPath, $"{fileNameBase}.png");

            try
            {
                await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = path,
                    FullPage = true
                });
                return path;
            }
            catch
            {
                return null;
            }
        }
    }

    }
