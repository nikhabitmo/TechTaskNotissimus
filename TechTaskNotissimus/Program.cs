using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
        var configFileStream = await File.ReadAllTextAsync("../../../Config.json");
        var parameters = JsonConvert.DeserializeObject<Config>(configFileStream);
        var address = parameters.Url;
        var city = parameters.City;
        var semaphore = new SemaphoreSlim(3);
        IProductInfoService productInfoService = new ProductInfoService(city);
        Console.WriteLine("Setup finished.");
        
        
        var driver = await LoadHtmlPage(address, city);
        var htmlContent = driver.PageSource;
        driver.Quit();

        
        Console.WriteLine("Parsing HTML.\n");
        var document = new HtmlParser().ParseDocument(htmlContent);
        Console.WriteLine("HTML Document was successfully parsed.\n");


        Console.WriteLine("Getting product links...");
        var productLinks = await productInfoService.ExtractProductLinks(document);
        Console.WriteLine("Product links were extracted.\n");


        Console.WriteLine("Collecting data... (it may take a few minutes)");
        var products = await productInfoService.GetProductsInfo(productLinks, semaphore);
        Console.WriteLine(products.ToList().Count);
        Console.WriteLine("Data was collected.\n");

        
        var json = JsonConvert.SerializeObject(products, Formatting.Indented);
        Console.WriteLine("Saving to json...");
        await File.WriteAllTextAsync($"WinesTest.json", json);
        Console.WriteLine("Saved to json.");
    }

    static async Task<ChromeDriver> LoadHtmlPage(string url, string? city)
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--disable-notifications");
        var driver = new ChromeDriver(options);
        driver.Navigate().GoToUrl(url);
        
        // 18+ confirm
        var ageConfirmationElement = driver.FindElement(By.CssSelector(".age-confirm__button.js-age-confirm"));
        ageConfirmationElement.Click();

        await Task.Delay(2000);

        // Loading page based on city in config
        if (string.Compare(city, "Москва", StringComparison.Ordinal) == 0)
        {
            var agreeButton = driver.FindElement(By.CssSelector(
                "button[data-autotest-target='city-popup'][data-autotest-target-id='city-popup-city-agree-btn']"));
            agreeButton.Click();
        }
        
        if (string.Compare(city, "Сочи", StringComparison.Ordinal) == 0)
        {
            var chooseAnotherButton = driver.FindElement(By.CssSelector(
                "button[data-autotest-target='city-popup'][data-autotest-target-id='city-popup-city-other-btn']"));
            chooseAnotherButton.Click();
            await Task.Delay(1500);
            var sochiLink = driver.FindElement(By.CssSelector(
                "a.location__link[href='/catalog/shampanskoe_i_igristoe_vino/?setVisitorCityId=5'][data-autotest-target='header-location-dropdown-city'][data-autotest-target-id='header-location-dropdown-city-3']"));
            sochiLink.Click();
        }
        
        
        var interestingSectionsTitleElement = driver.FindElement(By.CssSelector(
            "div.catalog-info__categories-title[data-autotest-target='catalog-interesting-title'][data-autotest-target-id='catalog-interesting-title']"));
        await Task.Delay(2000);
        
        // scrolling in order to load /page1, /page2, etc.
        Console.WriteLine("Loading all goods... Just chill for a minute");
        var scrollDown = true;
        var endTime = DateTime.Now.AddSeconds(69);
        while (DateTime.Now < endTime)
        {
            if (scrollDown)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);",
                    interestingSectionsTitleElement);
            }
            else
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, 0);");
            }

            scrollDown = !scrollDown;

            await Task.Delay(2000);
        }

        return driver;
    }
}