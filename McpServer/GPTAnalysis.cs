using System;

namespace McpServer;

public class GPTAnalysis
{
     public static string FormatDetailedReport(List<McpTools.TestResult> results, int passed, int failed)
    {
        var report = new System.Text.StringBuilder();
        
        report.AppendLine("╔════════════════════════════════════════════════════════════════╗");
        report.AppendLine("║           QA TEST EXECUTION - DETAILED RESULTS                 ║");
        report.AppendLine("╚════════════════════════════════════════════════════════════════╝");
        report.AppendLine();
        report.AppendLine($"📊 SUMMARY: {passed} Passed | {failed} Failed | Total: {results.Count}");
        report.AppendLine($"⏱️  Executed at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine();
        report.AppendLine("".PadRight(70, '='));
        report.AppendLine();

        int scenarioNumber = 1;
        foreach (var result in results)
        {
            string status = result.Passed ? "✅ PASSED" : "❌ FAILED";
            string statusLine = result.Passed ? "━" : "═";
            
            report.AppendLine($"TEST {scenarioNumber}: {result.ScenarioName}");
            report.AppendLine("".PadRight(70, statusLine[0]));
            report.AppendLine($"Status: {status}");
            report.AppendLine($"Description: {result.Description}");
            report.AppendLine($"Duration: {result.Duration.TotalSeconds:F2} seconds");
            report.AppendLine();
            
            if (result.Passed)
            {
                report.AppendLine($"✓ Result: {result.Reason}");
                report.AppendLine("  All assertions passed. No issues detected.");
            }
            else
            {
                report.AppendLine($"✗ Failure Reason: {result.Reason}");
                report.AppendLine();
                report.AppendLine("📋 Detailed Analysis:");
                report.AppendLine(FailureAnalyzier.AnalyzeFailure(result));
            }
            
            report.AppendLine();
            report.AppendLine("".PadRight(70, '─'));
            report.AppendLine();
            scenarioNumber++;
        }

        report.AppendLine("".PadRight(70, '='));
        report.AppendLine();
        report.AppendLine("🎯 RECOMMENDATIONS:");
        report.AppendLine(GenerateRecommendations(results));
        report.AppendLine();
        report.AppendLine("📄 Full HTML report available at: "+$"{Path.Combine(AppContext.BaseDirectory, "Reports")}");
        
        return report.ToString();
    }

    public static string GenerateRecommendations(List<McpTools.TestResult> results)
    {
        var recommendations = new System.Text.StringBuilder();
        bool hasSecurityIssue = results.Any(r => r.Reason.Contains("SECURITY"));
        bool hasValidationIssue = results.Any(r => r.Reason.Contains("VALIDATION"));
        bool hasTimeoutIssue = results.Any(r => r.Reason.Contains("TIMEOUT"));
        
        if (results.All(r => r.Passed))
        {
            recommendations.AppendLine("  ✓ All tests passed successfully!");
            recommendations.AppendLine("  ✓ Login functionality is working as expected");
            recommendations.AppendLine("  ✓ Security validations are in place");
        }
        else
        {
            if (hasSecurityIssue)
            {
                recommendations.AppendLine("  🔴 CRITICAL: Address security vulnerabilities immediately");
                recommendations.AppendLine("     - Review authentication logic");
                recommendations.AppendLine("     - Implement proper credential validation");
            }
            
            if (hasValidationIssue)
            {
                recommendations.AppendLine("  🟡 HIGH: Implement form validation");
                recommendations.AppendLine("     - Add required field checks");
                recommendations.AppendLine("     - Display user-friendly error messages");
            }
            
            if (hasTimeoutIssue)
            {
                recommendations.AppendLine("  🟠 MEDIUM: Investigate performance issues");
                recommendations.AppendLine("     - Check page load times");
                recommendations.AppendLine("     - Update test selectors if needed");
            }
        }
        
        return recommendations.ToString();
    }
}
