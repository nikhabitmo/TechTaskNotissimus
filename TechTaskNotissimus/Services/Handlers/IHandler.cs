using AngleSharp.Html.Dom;

namespace TechTaskNotissimus.Services.Handlers;

public interface IHandler<T>
{
    public void Handle(IHtmlDocument document);
}