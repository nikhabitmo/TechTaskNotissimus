using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using TechTaskNotissimus.Models;

namespace TechTaskNotissimus.Services;

public interface IProductInfoService
{
    public Task<IEnumerable<ProductBase>> GetProductsInfo(IEnumerable<string> productLinks, SemaphoreSlim semaphore);
    public Task<List<string>> ExtractProductLinks(IHtmlDocument document);
    protected Task<ProductBase> ProcessProductInfoAsync(string url, SemaphoreSlim semaphore);

    protected Task<string> GetProductName(IHtmlDocument document);
    protected Task<int> GetArticul(IHtmlDocument document);
    protected Task<(int oldPrice, int newPrice)> GetPrices(IHtmlDocument document);
    protected Task<double> GetRating(IHtmlDocument document);
    protected Task<float> GetVolume(IHtmlDocument document);
    protected Task<string> GetCityName(IHtmlDocument document);
    protected Task<List<string>> GetImageUrls(IHtmlDocument document);
}