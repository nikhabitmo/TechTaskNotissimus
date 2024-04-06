using System.Collections.Generic;

namespace TechTaskNotissimus.Models;

public abstract class ProductBase
{
    protected ProductBase(string name, int articul, int basePrice, int discountPrice, double rating, string city, string url, IList<string> picturesUrl)
    {
        Name = name;
        Articul = articul;
        BasePrice = basePrice;
        DiscountPrice = discountPrice;
        Rating = rating;
        City = city;
        Url = url;
        PicturesUrl = picturesUrl;
    }

    public string Name { get; private set; }
    public int Articul { get; private set; }
    public int BasePrice { get; private set; }
    public int DiscountPrice { get; private set; }
    public double Rating { get; private set; }
    public string City { get; private set; }
    public string Url { get; private set; }
    public IList<string> PicturesUrl { get; private set; }
}