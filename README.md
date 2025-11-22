MCP Automation Server (C# + Playwright + ExtentReports)

This project is an MCP (Model Context Protocol) automation server built using C#, designed to allow AI assistants (such as Claude, ChatGPT, or other MCP-compatible clients) to run real browser automation and testing tasks.

Using this server, an AI can trigger automated browser actions (login, UI navigation, validation, etc.) and receive results, logs, and screenshots.

🚀 Features

🔹 MCP Server Integration (works with Claude / ChatGPT MCP mode)

🔹 Playwright Browser Automation

Headed & headless execution

Page navigation, filling forms, clicking elements

🔹 ExtentReports HTML Reporting

Step logging

Pass/Fail result tracking

Screenshot attachment on failures


🛠️ Requirements

Component

.NET	8.0 or higher

Playwright	Latest

Microsoft MCP	Installed (Claude / VSCode extension)

Google Chrome installed	Required for automation

🧪 Available Tools


sayHello:	    Returns a simple greeting response

browserLogin:	Launches a browser, navigates to a URL, performs login, and logs results

list_tools:	  Returns all available MCP tools
