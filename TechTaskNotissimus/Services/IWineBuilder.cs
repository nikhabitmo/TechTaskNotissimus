using System.Collections.Generic;
using TechTaskNotissimus.Models;

namespace TechTaskNotissimus.Services;

public interface IWineBuilder
{
    IWineBuilder SetName(string name);
    IWineBuilder SetArticul(int articul);
    IWineBuilder SetBasePrice(int basePrice);
    IWineBuilder SetDiscountPrice(int discountPrice);
    IWineBuilder SetRating(double rating);
    IWineBuilder SetCity(string city);
    IWineBuilder SetUrl(string url);
    IWineBuilder SetVolume(float volume);
    IWineBuilder SetPicturesUrl(IList<string> picturesUrls);
    Wine Build();
}