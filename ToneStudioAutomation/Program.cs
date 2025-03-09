using System.Text.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;

// 1. Powershell : cd "C:\Program Files (x86)\BOSS\BOSS TONE STUDIO for KATANA MkII"
//      |_ .\"BOSS TONE STUDIO for KATANA MkII.exe" --remote-debugging-port=9223
// 2.    |_cd "C:\Program Files\Google\Chrome\Application"
//          |_ .\chrome.exe --remote-debugging-port=9223 --user-data-dir="C:\ChromeDebug"
string devToolsUrl = "http://localhost:9223/json"; // DevTools API endpoint

using (HttpClient client = new HttpClient())
{
    try
    {
        string response = await client.GetStringAsync(devToolsUrl);
        var sessions = JsonSerializer.Deserialize<JsonElement>(response);

        foreach (var session in sessions.EnumerateArray())
        {
            // Returns
            // [ {
            //     "description": "",
            //     "devtoolsFrontendUrl": "/devtools/inspector.html?ws=localhost:9223/devtools/page/E418E65EA0C405E7505357A3E9B5D5FB",
            //     "id": "E418E65EA0C405E7505357A3E9B5D5FB",
            //     "title": "BOSS TONE STUDIO for KATANA Mk II",
            //     "type": "page",
            //     "url": "file:///C:/Users/Josiah Hollibaugh/AppData/Local/Roland/BOSS TONE STUDIO for KATANA MkII/html/index.html",
            //     "webSocketDebuggerUrl": "ws://localhost:9223/devtools/page/E418E65EA0C405E7505357A3E9B5D5FB"
            // } ]
            
                string debuggerUrl = session.GetProperty("webSocketDebuggerUrl").GetString();
                
                
                var options = new ChromeOptions();
                options.DebuggerAddress = debuggerUrl;
                using (ChromeDriver driver = new ChromeDriver(options))
                {
                    // Execute Chrome DevTools Protocol Command to get the DOM tree
                    var toneStudioTree = driver.ExecuteCdpCommand("DOM.getDocument", new Dictionary<string, object>
                    {
                        { "depth", -1 }  // -1 means full depth
                    });

                    File.WriteAllText("C:\\Users\\Josiah Hollibaugh\\Documents\\KTNAOUT\rslt.txt", toneStudioTree.ToString());
                }
            
            
        }

        Console.WriteLine("No active session found for Katana Tone Studio.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error fetching DevTools sessions: " + ex.Message);
    }
}


// ChromeOptions options = new ChromeOptions();
// options.UseWebSocketUrl = true;


//using (ChromeDriver driver = new ChromeDriver(options))
//{
// driver.Navigate().GoToUrl("devtools://devtools/bundled/inspector.html?remoteFrontend=true");
// Get the DevTools session
//var devTools = (driver as IDevTools).GetDevToolsSession();
// var ses = devTools.GetVersionSpecificDomains<DevToolsSessionDomains>(); 
// Enable the console events
// devTools.Runtime.ConsoleAPICalled += (sender, e) =>
// {
//     Console.WriteLine($"Console output: {e.Message.Text}");
// };
// Inject JavaScript into the DevTools console
// string script = "console.log('JavaScript executed in DevTools Console');";
// devTools.Runtime.Evaluate(new EvaluateCommand
// {
//     Expression = script
// });
// This will log "JavaScript executed in DevTools Console" in your output
// Console.ReadLine();
//}


// Att 2 : chrome.exe --remote-debugging-port=9222 --user-data-dir="C:\ChromeDebug"