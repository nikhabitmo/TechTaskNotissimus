using System.Collections.Generic;
using TechTaskNotissimus.Models;

namespace TechTaskNotissimus.Services;

public class WineBuilder : IWineBuilder
{
    private string Name;
    private int Articul;
    private int BasePrice;
    private int DiscountPrice;
    private double Rating;
    private string City;
    private string Url;
    private float Volume;
    private IList<string> PicturesUrl;

    public IWineBuilder SetName(string name)
    {
        Name = name;
        return this;
    }

    public IWineBuilder SetArticul(int articul)
    {
        Articul = articul;
        return this;
    }

    public IWineBuilder SetBasePrice(int basePrice)
    {
        BasePrice = basePrice;
        return this;
    }

    public IWineBuilder SetDiscountPrice(int discountPrice)
    {
        DiscountPrice = discountPrice;
        return this;
    }

    public IWineBuilder SetRating(double rating)
    {
        Rating = rating;
        return this;
    }

    public IWineBuilder SetCity(string city)
    {
        City = city;
        return this;
    }

    public IWineBuilder SetUrl(string url)
    {
        Url = url;
        return this;
    }

    public IWineBuilder SetVolume(float volume)
    {
        Volume = volume;
        return this;
    }

    public IWineBuilder SetPicturesUrl(IList<string> picturesUrls)
    {
        PicturesUrl = picturesUrls;
        return this;
    }

    public Wine Build()
    {
        return new Wine(Name, Articul, BasePrice, DiscountPrice, Rating, City, Url, PicturesUrl, Volume);
    }
}