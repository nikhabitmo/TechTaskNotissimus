using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using TechTaskNotissimus.Models;
using TechTaskNotissimus.Services.Handlers;

namespace TechTaskNotissimus.Services;

public class ProductInfoService : IProductInfoService
{
    public ProductInfoService(string city)
    {
        ConfigCity = city;
    }

    public string ConfigCity { get; set; }

    public async Task<ProductBase> ProcessProductInfoAsync(string url, SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();

        try
        {
            return await GetProductInfo(url);
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task<List<string>> ExtractProductLinks(IHtmlDocument document)
    {
        var productLinksHandler = new ProductLinksHandler();
        return await productLinksHandler.Handle(document);
    }

    public Task<string> GetProductName(IHtmlDocument document)
    {
        return Task.FromResult(document.QuerySelector(".product-page__header")?.TextContent.Trim());
    }

    public Task<int> GetArticul(IHtmlDocument document)
    {
        var articulElement = document.QuerySelector(".product-page__article.js-copy-article");
        var articulText = articulElement?.TextContent.Trim();
        var articulNumberText = Regex.Match(articulText, @"\d+").Value;
        return Task.FromResult(Convert.ToInt32(articulNumberText));
    }

    public async Task<(int oldPrice, int newPrice)> GetPrices(IHtmlDocument document)
    {
        var oldPrice = 0;
        var newPrice = 0;
        var oldPriceElement = document.QuerySelector(".product-buy__old-price.product-buy__with-one");
        var newPriceElement = document.QuerySelector(".product-buy__price.product-buy__price-discount");

        if (oldPriceElement != null && newPriceElement != null)
        {
            oldPrice = await ExtractPrice(oldPriceElement.TextContent.Trim());
            newPrice = await ExtractPrice(newPriceElement.TextContent.Trim());
        }
        else
        {
            var priceElement = document.QuerySelector(".product-buy__price");
            if (priceElement != null)
            {
                oldPrice = newPrice = await ExtractPrice(priceElement.TextContent.Trim());
            }
        }

        return (oldPrice, newPrice);
    }

    private Task<int> ExtractPrice(string priceText)
    {
        var priceDigitsMatch = Regex.Match(priceText, @"\d+(\s\d+)?");
        if (priceDigitsMatch.Success)
        {
            var priceDigits = Regex.Replace(priceDigitsMatch.Value, @"\s", "");
            if (!string.IsNullOrEmpty(priceDigits) && int.TryParse(priceDigits, out int price))
            {
                return Task.FromResult(price);
            }
        }

        return Task.FromResult(0);
    }

    public Task<double> GetRating(IHtmlDocument document)
    {
        var ratingElement = document.QuerySelector(".rating-stars__value");
        var ratingText = ratingElement?.TextContent.Trim();
        if (double.TryParse(ratingText, out double rating))
        {
            return Task.FromResult(rating);
        }

        return Task.FromResult<double>(0);
    }

    public Task<float> GetVolume(IHtmlDocument document)
    {
        var volumeLink =
            document.QuerySelector(
                "dd.product-brief__value a[href*='/catalog/shampanskoe_i_igristoe_vino/filter/volume']");
        if (volumeLink != null)
        {
            var volumeText = volumeLink.TextContent.Trim();
            var volume = float.Parse(volumeText.Substring(0, volumeText.Length - 2));
            return Task.FromResult(volume);
        }

        return Task.FromResult<float>(0);
    }

    public Task<string> GetCityName(IHtmlDocument document)
    {
        if (string.Compare(ConfigCity, "Москва", StringComparison.Ordinal) == 0)
        {
            var cityLink =
                document.QuerySelector(
                    "a.location__link[data-autotest-target='header-location-dropdown-city'][data-autotest-target-id='header-location-dropdown-city-1']");
            return Task.FromResult(cityLink?.TextContent.Trim());
        }

        if (string.Compare(ConfigCity, "Сочи", StringComparison.Ordinal) == 0)
        {
            var cityLink =
                document.QuerySelector(
                    "a.location__link[data-autotest-target='header-location-dropdown-city'][data-autotest-target-id='header-location-dropdown-city-3']");
            return Task.FromResult(cityLink?.TextContent.Trim());
        }

        return Task.FromResult("");
    }

    public async Task<IEnumerable<ProductBase>> GetProductsInfo(IEnumerable<string> productLinks,
        SemaphoreSlim semaphore)
    {
        var tasks = productLinks.Select(link => ProcessProductInfoAsync(link, semaphore)).Distinct();
        return await Task.WhenAll(tasks);
    }


    public Task<List<string>> GetImageUrls(IHtmlDocument document)
    {
        var imageUrls = new List<string>();
        var imageElements = document.QuerySelectorAll("img.product-slider__slide-img");
        foreach (var element in imageElements)
        {
            var imageUrl = element.GetAttribute("src");
            var index = imageUrl.IndexOf('@');
            if (index != -1)
            {
                imageUrl = imageUrl.Substring(0, index);
            }

            if (!string.IsNullOrEmpty(imageUrl))
            {
                imageUrls.Add(imageUrl);
            }
        }

        return Task.FromResult(imageUrls);
    }

    private async Task<ProductBase> GetProductInfo(string url)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(url) as IHtmlDocument;

        var productName = await GetProductName(document);
        var articul = await GetArticul(document);
        var prices = await GetPrices(document);
        var rating = await GetRating(document);
        var volume = await GetVolume(document);
        var cityName = await GetCityName(document);
        var imageUrls = await GetImageUrls(document);

        var wine = new WineBuilder()
            .SetName(productName)
            .SetArticul(articul)
            .SetRating(Convert.ToDouble(rating))
            .SetBasePrice(prices.oldPrice)
            .SetDiscountPrice(prices.newPrice)
            .SetUrl(url)
            .SetPicturesUrl(imageUrls)
            .SetVolume((float)Convert.ToDouble(volume))
            .SetCity(cityName)
            .Build();

        return wine;
    }
}