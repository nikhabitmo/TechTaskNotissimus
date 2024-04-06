using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;
using TechTaskNotissimus.Models;
using TechTaskNotissimus.Services;

namespace TechTaskNotissimus;

public static class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Setup...");
        var configFileStream = File.ReadAllText("../../../Config.json");
        var parameters = JsonConvert.DeserializeObject<Config>(configFileStream);
        var address = parameters.Url;
        var city = parameters.City;

        var semaphore = new SemaphoreSlim(3);
        IProductInfoService productInfoService = new ProductInfoService(city);
        
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        var driver = new ChromeDriver(options);
        driver.Navigate().GoToUrl(address);
        
        var ageConfirmationElement = driver.FindElement(By.CssSelector(".age-confirm__button.js-age-confirm"));
        ageConfirmationElement.Click();
        
        await Task.Delay(2000);

        var agreeButton = driver.FindElement(By.CssSelector("button[data-autotest-target='city-popup'][data-autotest-target-id='city-popup-city-agree-btn']"));
        agreeButton.Click();

        var interestingSectionsTitleElement = driver.FindElement(By.CssSelector("div.catalog-info__categories-title[data-autotest-target='catalog-interesting-title'][data-autotest-target-id='catalog-interesting-title']"));


        Console.WriteLine("Loading all goods... Just chill for a minute");
        bool scrollDown = true;
        var endTime = DateTime.Now.AddSeconds(60);
        while (DateTime.Now < endTime)
        {
            if (scrollDown)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", interestingSectionsTitleElement);
            }
            else
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, 0);");
            }

            scrollDown = !scrollDown;

            await Task.Delay(2000);
        }
        

        var htmlContent = driver.PageSource;

        driver.Quit();
        
        
        var parser = new HtmlParser();
        var document = parser.ParseDocument(htmlContent);

        Console.WriteLine("HTML Document was successfully opened.\n");


        Console.WriteLine("Getting product links...");
        var productLinks = productInfoService.ExtractProductLinks(document);
        Console.WriteLine("Product links were extracted.\n");


        Console.WriteLine("Collecting data... (it may take few minutes)");
        var products = await productInfoService.GetProductsInfo(productLinks, semaphore);
        Console.WriteLine("Data was collected.\n");

        var json = JsonConvert.SerializeObject(products.ToHashSet(), Formatting.Indented);
        Console.WriteLine("Saving to json...");
        await File.WriteAllTextAsync("products.json", json);
        Console.WriteLine("Saved to json.");
    }
}