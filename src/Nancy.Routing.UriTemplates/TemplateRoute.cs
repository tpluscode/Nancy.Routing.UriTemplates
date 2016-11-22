using System;

namespace Nancy.Routing.UriTemplates
{
    public abstract class TemplateRoute : Route
    {
        protected TemplateRoute(string name, string method, string templateString, Func<NancyContext, bool> condition)
            : base(name, method, templateString, condition)
        {
            this.Template = new TunnelVisionLabs.Net.UriTemplate(templateString);
        }

        public TunnelVisionLabs.Net.UriTemplate Template { get; private set; }
    }
}