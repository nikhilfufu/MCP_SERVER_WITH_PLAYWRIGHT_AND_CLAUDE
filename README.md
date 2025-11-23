# 🤖 QA Testing MCP Server

> **AI-Powered Automated QA Testing** with Claude and Playwright

A Model Context Protocol (MCP) server that enables Claude to perform intelligent, automated QA testing on web applications. Get detailed test reports, AI-powered failure analysis, and actionable recommendations - all through natural conversation with Claude.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Playwright](https://img.shields.io/badge/Playwright-Latest-green.svg)](https://playwright.dev/)

---

## ✨ Features

### 🎯 **Comprehensive Test Scenarios**
- ✅ **Valid Login Testing** - Verify successful authentication flows
- ❌ **Invalid Credentials Testing** - Ensure security measures work correctly
- 📝 **Form Validation Testing** - Test empty field handling and validation
- 🔄 **Batch Testing** - Run all scenarios sequentially with detailed reporting

### 📊 **Intelligent Reporting**
- **Detailed Pass/Fail Analysis** - Know exactly what passed and what failed
- **Root Cause Identification** - AI-powered failure analysis
- **Impact Assessment** - Understand the severity of each issue
- **Actionable Recommendations** - Get specific steps to fix problems
- **Automated Screenshots** - Capture failures for debugging

### 🚀 **Built for Claude**
- **Natural Language Interface** - Ask Claude to run tests conversationally
- **Contextual Understanding** - Claude interprets results and provides insights
- **Integrated Workflow** - Seamless testing within your Claude conversations

---

## 📋 Table of Contents

- [Prerequisites](#-prerequisites)
- [Installation](#-installation)
- [Configuration](#-configuration)
- [Usage](#-usage)
- [Available Tools](#-available-tools)
- [Example Output](#-example-output)
- [Customization](#-customization)
- [Troubleshooting](#-troubleshooting)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🔧 Prerequisites

Before you begin, ensure you have the following installed:

- **[.NET 8.0 SDK](https://dotnet.microsoft.com/download)** or higher
- **[Claude Desktop](https://claude.ai/download)** application
- **[Playwright](https://playwright.dev/)** (installed automatically during setup)
- **Windows, macOS, or Linux** operating system

---

## 📦 Installation

### Step 1: Clone the Repository

```bash
git clone https://github.com/yourusername/qa-testing-mcp.git
cd qa-testing-mcp
```

### Step 2: Restore Dependencies

```bash
dotnet restore
```

### Step 3: Install Playwright Browsers

```bash
# Install Playwright CLI globally
dotnet tool install --global Microsoft.Playwright.CLI

# Install Chromium browser
playwright install chromium
```

### Step 4: Build the Project

```bash
dotnet build -c Release
```

### Step 5: Test the Installation

```bash
dotnet run --project QA_MCP.csproj
```

You should see the MCP server start up successfully.

---

## ⚙️ Configuration

### Configure Claude Desktop

1. **Locate your Claude Desktop config file:**

   - **Windows:** `%APPDATA%\Claude\claude_desktop_config.json`
   - **macOS:** `~/Library/Application Support/Claude/claude_desktop_config.json`
   - **Linux:** `~/.config/Claude/claude_desktop_config.json`

2. **Add the MCP server configuration:**

```json
{
  "mcpServers": {
    "qa-testing": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\absolute\\path\\to\\qa-testing-mcp\\QA_MCP.csproj"
      ]
    }
  }
}
```

> **⚠️ Important:** Use the **absolute path** to your project directory.

**Example Configurations:**

**Windows:**
```json
{
  "mcpServers": {
    "qa-testing": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Users\\YourName\\Projects\\qa-testing-mcp\\QA_MCP.csproj"
      ]
    }
  }
}
```

**macOS/Linux:**
```json
{
  "mcpServers": {
    "qa-testing": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/Users/yourname/projects/qa-testing-mcp/QA_MCP.csproj"
      ]
    }
  }
}
```

### Step 3: Restart Claude Desktop

1. **Quit Claude Desktop completely** (not just close the window)
2. **Reopen Claude Desktop**
3. Look for the 🔌 **tool icon** in the input box

---

## 🚀 Usage

### Quick Start

Once configured, you can interact with the QA testing tools through natural conversation with Claude:

```
Hey Claude, can you test the login functionality on https://myapp.com 
with username "testuser" and password "testpass"?
```

Claude will automatically call the appropriate testing tools and provide detailed analysis.

### Direct Tool Invocation

You can also directly invoke specific test scenarios:

#### **Test Valid Login**
```
Run scenario1_ValidLogin with:
- url: "https://myapp.com/login"
- username: "testuser"
- password: "testpass123"
```

#### **Test Invalid Login**
```
Run scenario2_InvalidLogin with:
- url: "https://myapp.com/login"  
- wrongUsername: "baduser"
- wrongPassword: "wrongpass"
```

#### **Test Empty Fields**
```
Run scenario3_EmptyFields with:
- url: "https://myapp.com/login"
```

#### **Run All Tests**
```
Run runAllScenarios with:
- url: "https://myapp.com/login"
- validUsername: "testuser"
- validPassword: "testpass123"
- invalidUsername: "baduser"
- invalidPassword: "wrongpass"
```

---

## 🛠️ Available Tools

| Tool | Description | Parameters |
|------|-------------|------------|
| `sayHello` | Test MCP connection | `name` (string) |
| `list_tools` | List all available tools | None |
| `scenario1_ValidLogin` | Test successful login flow | `url`, `username`, `password` |
| `scenario2_InvalidLogin` | Test invalid credentials rejection | `url`, `wrongUsername`, `wrongPassword` |
| `scenario3_EmptyFields` | Test empty field validation | `url` |
| `runAllScenarios` | Execute all test scenarios | `url`, `validUsername`, `validPassword`, `invalidUsername`, `invalidPassword` |

---

## 📊 Example Output

When you run `runAllScenarios`, you'll receive a comprehensive report like this:

```
╔════════════════════════════════════════════════════════════════╗
║           QA TEST EXECUTION - DETAILED RESULTS                 ║
╚════════════════════════════════════════════════════════════════╝

📊 SUMMARY: 2 Passed | 1 Failed | Total: 3
⏱️  Executed at: 2024-11-23 14:30:45

======================================================================

TEST 1: Scenario 1: Valid Login Test
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Status: ✅ PASSED
Description: Tests that a valid user can successfully log in with correct credentials
Duration: 3.45 seconds

✓ Result: All validations passed successfully
  All assertions passed. No issues detected.

──────────────────────────────────────────────────────────────────────

TEST 2: Scenario 2: Invalid Login Test
══════════════════════════════════════════════════════════════════════
Status: ❌ FAILED
Description: Tests that invalid credentials are properly rejected by the system
Duration: 2.87 seconds

✗ Failure Reason: SECURITY ISSUE: System accepted invalid credentials

📋 Detailed Analysis:
  • Invalid credentials were accepted by the system
  • This is a critical security vulnerability
  • Impact: Unauthorized access possible
  • Action Required: Fix authentication validation immediately

──────────────────────────────────────────────────────────────────────

TEST 3: Scenario 3: Empty Fields Validation
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Status: ✅ PASSED
Description: Tests that the login form validates empty username/password fields
Duration: 1.23 seconds

✓ Result: All validations passed successfully
  All assertions passed. No issues detected.

──────────────────────────────────────────────────────────────────────

======================================================================

🎯 RECOMMENDATIONS:
  🔴 CRITICAL: Address security vulnerabilities immediately
     - Review authentication logic
     - Implement proper credential validation

📄 Full HTML report available at: ExtentReport.html
```

### Claude's AI-Powered Analysis

After running tests, Claude will provide intelligent insights like:

> "I've completed the QA testing on your login page. Here's what I found:
> 
> **Good News:** Your valid login flow works perfectly (3.45s), and empty field validation is properly implemented.
> 
> **Critical Issue:** There's a serious security vulnerability - the system is accepting invalid credentials. This needs immediate attention as it could allow unauthorized access to your application.
> 
> **Recommendation:** I suggest reviewing your authentication logic in the backend. The issue is likely in the credential validation step. Would you like me to help you investigate this further?"

---

## 🎨 Customization

### Modify Test Selectors

Edit the selectors in `Program.cs` to match your application:

```csharp
// Current selectors
await page.FillAsync("#txtUsername", username);
await page.FillAsync("#txtPassword", password);
await page.ClickAsync("#pre-login-btn");
await page.WaitForSelectorAsync("#grid-Gather");

// Change to your selectors
await page.FillAsync("[name='email']", username);
await page.FillAsync("[name='password']", password);
await page.ClickAsync("button[type='submit']");
await page.WaitForSelectorAsync(".dashboard");
```

### Add New Test Scenarios

Create additional test methods in the `McpTools` class:

```csharp
[McpServerTool, Description("Test password reset functionality")]
public static async Task<string> scenario4_PasswordReset(string url, string email)
{
    var test = ReportManager.CreateTest1("Scenario 4 - Password Reset");
    
    // Your test logic here
    
    return "Test result";
}
```

### Configure Timeouts and Delays

Adjust timeout settings:

```csharp
// Change timeout
await page.WaitForSelectorAsync("#element", 
    new PageWaitForSelectorOptions { Timeout = 15000 }); // 15 seconds

// Change delay between scenarios
await Task.Delay(3000); // 3 seconds
```

### Enable/Disable Headless Mode

```csharp
browser = await playwright.Chromium.LaunchAsync(
    new BrowserTypeLaunchOptions { 
        Headless = true  // Change to true for headless mode
    });
```

---

## 🐛 Troubleshooting

### MCP Not Appearing in Claude

**Problem:** Tool icon doesn't show up in Claude Desktop

**Solutions:**
1. Verify the config file path is correct
2. Ensure JSON syntax is valid (use a JSON validator)
3. Use absolute paths, not relative paths
4. Check Claude Desktop logs:
   - Windows: `%APPDATA%\Claude\logs`
   - macOS: `~/Library/Logs/Claude`
   - Linux: `~/.config/Claude/logs`
5. Completely restart Claude Desktop

### Playwright Browser Not Found

**Problem:** Error: "Executable doesn't exist at..."

**Solution:**
```bash
playwright install chromium
```

### Test Fails with Timeout

**Problem:** "Timeout 10000ms exceeded"

**Solutions:**
1. Increase timeout in code
2. Check if the target website is loading slowly
3. Verify selectors are correct
4. Ensure your internet connection is stable

### Selectors Not Found

**Problem:** "No element matches selector..."

**Solutions:**
1. Inspect the webpage and verify selector IDs/classes
2. Update selectors in the code to match your application
3. Wait for page to load completely before interacting
4. Use more specific selectors

### Build Errors

**Problem:** Build fails with dependency errors

**Solution:**
```bash
dotnet clean
dotnet restore
dotnet build
```

---

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. **Fork the repository**
2. **Create a feature branch:** `git checkout -b feature/amazing-feature`
3. **Commit your changes:** `git commit -m 'Add amazing feature'`
4. **Push to the branch:** `git push origin feature/amazing-feature`
5. **Open a Pull Request**

### Ideas for Contributions

- 🌐 Support for additional browsers (Firefox, Safari)
- 📱 Mobile testing capabilities
- 🔄 API testing integration
- 🎨 Custom reporting themes
- 🌍 Multi-language support
- ⚡ Parallel test execution
- 📊 Performance metrics tracking

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- **[Anthropic](https://anthropic.com)** - For Claude and the MCP protocol
- **[Playwright](https://playwright.dev/)** - For browser automation
- **[ExtentReports](https://www.extentreports.com/)** - For test reporting

---

## 📞 Support

- **Issues:** [GitHub Issues](https://github.com/yourusername/qa-testing-mcp/issues)
- **Discussions:** [GitHub Discussions](https://github.com/yourusername/qa-testing-mcp/discussions)
- **Email:** your.email@example.com

---

## 🌟 Star History

If you find this project useful, please consider giving it a ⭐ on GitHub!

---

## 📚 Additional Resources

- [MCP Documentation](https://modelcontextprotocol.io)
- [Claude API Documentation](https://docs.anthropic.com)
- [Playwright Documentation](https://playwright.dev/docs/intro)
- [.NET Documentation](https://docs.microsoft.com/dotnet)

---