using System.Collections.Generic;

namespace Nancy.Routing.UriTemplates
{
    /// <summary>
    /// A Nancy module extended with support for URI Template routes
    /// </summary>
    /// <seealso cref="Nancy.INancyModule" />
    public interface IUriTemplateRouting : INancyModule
    {
        /// <summary>
        /// Gets the URI Template routes.
        /// </summary>
        IEnumerable<TemplateRoute> TemplateRoutes { get; }
    }
}