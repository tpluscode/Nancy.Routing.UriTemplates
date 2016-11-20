using System.Collections.Generic;

namespace Nancy.Routing.UriTemplate
{
    public interface IUriTemplateRouting : INancyModule
    {
        IEnumerable<Route> TemplateRoutes { get; }
    }
}