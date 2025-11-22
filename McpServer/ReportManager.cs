using System;
using System.Diagnostics;
using System.IO;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace Reporting
{
    public static class ReportManager
    {
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
            spark.Config.ReportName   = "MCP Automation Execution";

            var extent = new ExtentReports();
            extent.AttachReporter(spark);

            extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
            extent.AddSystemInfo("Machine", Environment.MachineName);
            extent.AddSystemInfo(".NET", Environment.Version.ToString());

            _reportPath = reportPath;
            return extent;
        });

        private static string? _reportPath;
        public static ExtentReports Extent => _lazyExtent.Value;

        // Create test
        public static AventStack.ExtentReports.ExtentTest CreateTest1(string testName)
        {
            return Extent.CreateTest(testName);
        }

        public static void FlushAndOpen()
        {
            Extent.Flush();

            try
            {
                if (!string.IsNullOrWhiteSpace(_reportPath) && File.Exists(_reportPath))
                {
                    try
{
    var psi = new ProcessStartInfo
    {
        FileName = _reportPath,
        UseShellExecute = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true
    };

    using var proc = Process.Start(psi);
}
catch
{
    // ignore
}
                }
            }
            catch
            {
                // ignore – file still exists
            }
        }
    }
}
