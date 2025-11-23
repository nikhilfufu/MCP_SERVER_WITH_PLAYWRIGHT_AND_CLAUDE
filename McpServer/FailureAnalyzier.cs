public static class FailureAnalyzier
{
    public static string ExtractFailureReason(string output)
    {
        if (output.Contains("Invalid credentials were accepted"))
            return "SECURITY ISSUE: System accepted invalid credentials";
        if (output.Contains("Empty fields were not validated"))
            return "VALIDATION ISSUE: Form allowed submission with empty fields";
        if (output.Contains("timeout"))
            return "TIMEOUT: Expected element did not appear within time limit";
        if (output.Contains("selector"))
            return "ELEMENT NOT FOUND: Required page element is missing or has changed";
        
        return "Test failed - check output for details";
    }

    public static string AnalyzeFailure(McpTools.TestResult result)
    {
        var analysis = new System.Text.StringBuilder();
        
        if (result.Reason.Contains("SECURITY ISSUE"))
        {
            analysis.AppendLine("  • Invalid credentials were accepted by the system");
            analysis.AppendLine("  • This is a critical security vulnerability");
            analysis.AppendLine("  • Impact: Unauthorized access possible");
            analysis.AppendLine("  • Action Required: Fix authentication validation immediately");
        }
        else if (result.Reason.Contains("VALIDATION ISSUE"))
        {
            analysis.AppendLine("  • Form submitted without required field values");
            analysis.AppendLine("  • Client-side validation is missing or bypassed");
            analysis.AppendLine("  • Impact: Poor user experience, potential errors");
            analysis.AppendLine("  • Action Required: Implement proper field validation");
        }
        else if (result.Reason.Contains("TIMEOUT"))
        {
            analysis.AppendLine("  • Expected page element did not load within timeout period");
            analysis.AppendLine("  • Possible causes: Slow server response, changed selectors");
            analysis.AppendLine("  • Impact: Test cannot verify functionality");
            analysis.AppendLine("  • Action Required: Check server performance and element selectors");
        }
        else if (result.Reason.Contains("ELEMENT NOT FOUND"))
        {
            analysis.AppendLine("  • Required page element could not be located");
            analysis.AppendLine("  • Possible causes: UI changes, incorrect selector, page not loaded");
            analysis.AppendLine("  • Impact: Cannot complete test execution");
            analysis.AppendLine("  • Action Required: Update selectors or verify page structure");
        }
        else
        {
            analysis.AppendLine($"  • {result.Output}");
            analysis.AppendLine("  • Review screenshots and logs for more details");
        }
        
        return analysis.ToString();
    }
}