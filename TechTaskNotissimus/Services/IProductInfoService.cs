using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using TechTaskNotissimus.Models;

namespace TechTaskNotissimus.Services;

public interface IProductInfoService
{
    public Task<IEnumerable<ProductBase>> GetProductsInfo(IEnumerable<string> productLinks, SemaphoreSlim semaphore);
    public List<string> ExtractProductLinks(IHtmlDocument document);
    protected Task<ProductBase> ProcessProductInfoAsync(string url, SemaphoreSlim semaphore);

    protected string GetProductName(IHtmlDocument document);
    protected int GetArticul(IHtmlDocument document);
    protected (int oldPrice, int newPrice) GetPrices(IHtmlDocument document);
    protected double GetRating(IHtmlDocument document);
    protected float GetVolume(IHtmlDocument document);
    protected string GetCityName(IHtmlDocument document);
    protected List<string> GetImageUrls(IHtmlDocument document);
}