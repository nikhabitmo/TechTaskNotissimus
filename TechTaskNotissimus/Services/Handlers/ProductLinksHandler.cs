using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;

namespace TechTaskNotissimus.Services.Handlers;

public class ProductLinksHandler : IHandler<List<string>>
{
    public List<string> ProductLinks { get; private set; }

    public void Handle(IHtmlDocument document)
    {
        var baseUri = new Uri("https://simplewine.ru");
        ProductLinks = document.QuerySelectorAll("a[href^='/catalog/product/']")
            .Select(a => new Uri(baseUri, a.GetAttribute("href")).AbsoluteUri)
            .Where(link => !link.EndsWith("/reviews/"))
            .Where(link => !link.EndsWith("#stores"))
            .Distinct()
            .ToList();
    }
}
