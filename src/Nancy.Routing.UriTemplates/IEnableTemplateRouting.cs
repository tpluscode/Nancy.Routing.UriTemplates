using System;

namespace Nancy.Routing.UriTemplates
{
    public interface IEnableTemplateRouting : IDisposable
    {
        IDisposable Strict { get; }
    }
}