// See https://aka.ms/new-console-template for more information

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

Console.WriteLine("Hello, World!");

var options = new ChromeOptions();
options.DebuggerAddress = "localhost:9222"; // Attach to running Tone Studio
var driver = new ChromeDriver(, options);
using (var driver = new ChromeDriver(@"C:\Path\To\OldChromeDriver", options))
{
    try
    {
        var gainInput = driver.FindElement(By.CssSelector("#panel-amp-gain-spinner input"));
        string gainValue = gainInput.GetAttribute("value");

        Console.WriteLine($"Gain: {gainValue}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}