using System.Text.Json;
using PuppeteerSharp;

namespace AMPAI.Automation.Pupeteer;

public static class PuppeteerAutomation
{
    /*  TODO
         - Automate startup for chrome / dev tools / tone studio (currently using powershell:
                    1. .\"C:\Program Files (x86)\BOSS\BOSS TONE STUDIO for KATANA MkII\BOSS TONE STUDIO for KATANA MkII.exe" --remote-debugging-port={default is 9223}
                    2. .\"C:\Program Files\Google\Chrome\Application\chrome.exe" --disable-web-security --allow-file-access-from-files --disable-site-isolation-trials --disable-cors
                    3. Chrome needs port forwarding enabled (chrome://inspect/#devices should show "BOSS TONE STUDIO for KATANA Mk II" under "Remote Target")
                    *********************************************    EXAMPLE   ***************************************************************************** 
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "powershell",
                        Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"& './BOSS TONE STUDIO for KATANA MkII.exe' --remote-debugging-port=9222\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                     });
                     ****************************************************************************************************************************************
         - Implement event consumer to start / stop the service, collect output
         - Populate and output AMPAI.Core models (AmpSettingSnapshot); no file output here   
    */ 
    public static async Task RunAsync()
    {
        string wsUrl = "";
        string devToolsUrl = "http://localhost:9223/json"; // DevTools API endpoint
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
            // }]
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
            KeepAliveInterval =0000999999
        });
        var pages = await browser.PagesAsync();
        var page = pages[0]; // Select the correct tab
        
        bool hasJQuery = await page.EvaluateExpressionAsync<bool>("!!window.jQuery");
        if (!hasJQuery)
        {
            Console.WriteLine("Injecting jQuery...");
            await page.AddScriptTagAsync(new AddTagOptions { Url = "https://code.jquery.com/jquery-3.6.0.min.js" });
        }
        
        Console.WriteLine("Puppeteer connected to CEF devtools successfully - recording inputs every 5 seconds.");
        string outputPath = @"%AppData%\AMPAI\TSSnapshot.txt";
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