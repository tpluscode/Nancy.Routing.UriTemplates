using System;

namespace Nancy.Routing.UriTemplates
{
    [Flags]
    internal enum RoutingMode
    {
        Standard = 1,
        Templates = 2,
        Strict = 4
    }
}