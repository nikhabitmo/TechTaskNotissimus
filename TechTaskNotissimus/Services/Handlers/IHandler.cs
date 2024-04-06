using AngleSharp.Html.Dom;

namespace TechTaskNotissimus.Services.Handlers;

public interface IHandler<T>
{
    public Task<T> Handle(IHtmlDocument document);
}