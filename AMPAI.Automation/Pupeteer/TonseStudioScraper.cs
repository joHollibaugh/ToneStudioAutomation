using PuppeteerSharp;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ToneStudioAutomation;

public static class PuppeteerAutomation
{
    public static async Task RunAsync()
    {
        // Use PS to start tone studio - machine specific for path / version
        // Process.Start(new ProcessStartInfo
        // {
        //     FileName = "powershell",
        //     Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"& './BOSS TONE STUDIO for KATANA MkII.exe' --remote-debugging-port=9222\"",
        //     UseShellExecute = false,
        //     CreateNoWindow = true
        // });
        string wsUrl = "";
        string devToolsUrl = "http://localhost:9222/json"; // DevTools API endpoint
        using (HttpClient client = new HttpClient())
        {
            string response = await client.GetStringAsync(devToolsUrl);
            var sessions = JsonSerializer.Deserialize<JsonElement>(response);
            // Returns
            // [ {
            //     "description": "",
            //     "devtoolsFrontendUrl": "/devtools/inspector.html?ws=localhost:9223/devtools/page/E418E65EA0C405E7505357A3E9B5D5FB",
            //     "id": "E418E65EA0C405E7505357A3E9B5D5FB",
            //     "title": "BOSS TONE STUDIO for KATANA Mk II",
            //     "type": "page",
            //     "url": "file:///{$AppData}/Local/Roland/BOSS TONE STUDIO for KATANA MkII/html/index.html",
            //     "webSocketDebuggerUrl": "ws://localhost:9223/devtools/page/E418E65EA0C405E7505357A3E9B5D5FB"
            // } ]
            foreach (var session in sessions.EnumerateArray())
            {
                if (session.GetProperty("title").GetString() == "BOSS TONE STUDIO for KATANA Mk II")
                {
                    wsUrl = session.GetProperty("webSocketDebuggerUrl").GetString();
                    continue;
                }
            }
        }

        var browser = await Puppeteer.ConnectAsync(new ConnectOptions
        {
            IgnoreHTTPSErrors = true,
            BrowserWSEndpoint = wsUrl,
        });
        var pages = await browser.PagesAsync();
        var page = pages[0]; // Select the correct tab

        // Inject jQuery if it's not already present
        bool hasJQuery = await page.EvaluateExpressionAsync<bool>("!!window.jQuery");
        if (!hasJQuery)
        {
            Console.WriteLine("Injecting jQuery...");
            await page.AddScriptTagAsync(new AddTagOptions { Url = "https://code.jquery.com/jquery-3.6.0.min.js" });
        }

        // TODO - *replace with orchestrator context - publish event to cascade sync / stamp / write ops with recording 
        //          ** Handle session closed exceptions
        Console.WriteLine("Puppeteer connected to CEF devtools successfully - recording inputs every 5 seconds.");
        string outputPath = @"C:\Users\Josiah Hollibaugh\Documents\KTNAOUT\ToneStudioData.txt";
        using (StreamWriter writer = new StreamWriter(outputPath, append: true))
        {
            while (true)
            {
                try
                {
                    var result = await page.EvaluateExpressionAsync<string[]>(
                        @"$('#editor-panel-page input').map(function() { return $(this).val(); }).get();");
                    string output = string.Join(", ", result);
                    Console.WriteLine($"Successful Response: {output}");

                    await writer.WriteLineAsync($"{DateTime.UtcNow}: {output}");
                    await writer.FlushAsync();
                }
                catch (PuppeteerException ex)
                {
                    Console.WriteLine($"Puppeteer Exception occurred: {ex.Message}");
                }

                await Task.Delay(5000);
            }
        }
    }
}