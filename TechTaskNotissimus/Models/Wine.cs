using System.Collections.Generic;

namespace TechTaskNotissimus.Models;

public class Wine : ProductBase
{
    public float Volume { get; private set; }

    public Wine(string name, int articul, int basePrice, int discountPrice, double rating, string city, string url,
        IList<string> picturesUrl, float volume) : base(name, articul, basePrice, discountPrice, rating, city, url, picturesUrl)
    {
        Volume = volume;
    }
}