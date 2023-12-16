using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using RandomUserAgent;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();



static async Task randomScrollAsync(FirefoxDriver driver)
{
    Random random = new Random();
    int i = random.Next(0, 2);
    Console.WriteLine(i);
    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
    if (i == 0)
    {
        js.ExecuteScript($"window.scrollBy({random.Next(0, 750)},0)");
    }
    else
    {
        js.ExecuteScript($"window.scrollBy(0,{random.Next(250, 750)})");
    }
    await Task.Delay(random.Next(32000, 45001));
}

static async Task ViewAsync(string URL, string GEO)
{
    // string chromeDriverPath = "chromedriver";
    // var chromeOptions = new ChromeOptions();
    // string userAgent = RandomUa.RandomUserAgent;
    // chromeOptions.AddArgument("headless"); // Run Chrome in headless mode
    // chromeOptions.AddArgument($"user-agent={userAgent}");
    // chromeOptions.AddArgument("start-maximized");
    // chromeOptions.AddExcludedArgument("enable-automation");
    // chromeOptions.AddAdditionalOption("useAutomationExtension", false);
    string firefoxDriverPath = "geckodriver";
    var firefoxOptions = new FirefoxOptions();
    string userAgent = RandomUa.RandomUserAgent;
    firefoxOptions.AddArgument($"user-agent={userAgent}");
    firefoxOptions.AddArgument("start-maximized");
    Console.WriteLine("hello");
    try
    {

        // ChromeDriver driver = new(chromeOptions);
        FirefoxDriver driver = new(firefoxOptions);

        string url = URL;

        // Navigate to the specified URL
        driver.Navigate().GoToUrl(url);
        await Task.Delay(2000);

        Random random = new Random();
        int i = random.Next(5, 10);
        int o = 0;
        Console.WriteLine(i);
        while (o < i)
        {
            Console.WriteLine("o" + o);
            await randomScrollAsync(driver);
            o++;
        }
        driver.Close();
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}


app.MapGet("/", () => "Hello");

app.MapPost("/", async (HttpRequest request, BodyRequesti body) =>
{

    if (!request.Headers.ContainsKey("IX")) { return Results.Problem("Authorization token is missing.", statusCode: 401); }
    var authorizationHeader = request.Headers["IX"].ToString();
    if (authorizationHeader != "JXStncq0") { return Results.Problem("Invalid authorization token.", statusCode: 403); };
    var startTime = DateTime.Now;
    var endTime = startTime.AddMinutes(60);
    var timeInterval = TimeSpan.FromMinutes(60.0 / body.Views); // Calculate time interval between each bot creation
    var tasks = new List<Task>();

    while (DateTime.Now < endTime)
    {
        var task = ViewAsync(body.Url, body.Geo);
        tasks.Add(task);
        await Task.Delay(timeInterval);
    }

    await Task.WhenAll(tasks);

    return Results.Ok();
});

app.Run();


record BodyRequesti(string Url, int Views, string Geo = "SE");