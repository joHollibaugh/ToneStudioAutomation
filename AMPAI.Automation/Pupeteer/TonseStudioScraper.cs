using PuppeteerSharp;
using System;
using System.Threading.Tasks;

namespace ToneStudioAutomation;

public static class PuppeteerAutomation
{
    public static async Task RunAsync()
    {
        // Here, we specify that we want Chrome and set a custom download folder.string chromedriverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "chromedriver.exe");
        string chromedriverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "chromedriver.exe");

        var fetcherOptions = new BrowserFetcherOptions
        {
            Browser = SupportedBrowser.Chrome,
            // This path will be combined with your project base directory.
            Path = chromedriverPath
        };
        var browserFetcher = Puppeteer.CreateBrowserFetcher(fetcherOptions);
        //await browserFetcher.DownloadAsync(); // This will ensure Chrome is downloaded        var browserFetcher = Puppeteer.CreateBrowserFetcher(fetcherOptions);

        var launchOptions = new LaunchOptions
        {
            ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe", // Adjust if needed
            Headless = false, // Set to false to see the UI
            Args = new[]
            {
                "--allow-file-access-from-files",
                "--disable-web-security",
                "--disable-site-isolation-trials"
            }
        };

        var browser = await Puppeteer.LaunchAsync(launchOptions);


        var page = await browser.NewPageAsync();
        string fileUrl =
            "file:///C:/Users/Josiah%20Hollibaugh/AppData/Local/Roland/BOSS%20TONE%20STUDIO%20for%20KATANA%20MkII/html/index.html";
        await page.GoToAsync(fileUrl);
        Console.WriteLine("Navigated to Tone Studio page (Puppeteer).");

        // Inject jQuery if it's not already present
        bool hasJQuery = await page.EvaluateExpressionAsync<bool>("!!window.jQuery");
        if (!hasJQuery)
        {
            Console.WriteLine("Injecting jQuery...");
            await page.AddScriptTagAsync(new AddTagOptions { Url = "https://code.jquery.com/jquery-3.6.0.min.js" });
        }

        // Run a jQuery snippet to extract the GAIN value
        string gainValue = await page.EvaluateExpressionAsync<string>(@"(function(){
                    return $('#panel-amp-gain-spinner input').val();
                })()");
        Console.WriteLine($"Puppeteer: GAIN Value = {gainValue}");
    }
}