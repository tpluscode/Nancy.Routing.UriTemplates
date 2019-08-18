using System;

namespace Nancy.Routing.UriTemplates
{
    public abstract class TemplateRoute : Route
    {
        protected TemplateRoute(string name, string method, string templateString, Func<NancyContext, bool> condition, Type type)
            : base(name, method, templateString, condition, type)
        {
            this.Template = new UriTemplate.Core.UriTemplate(templateString);
        }

        public UriTemplate.Core.UriTemplate Template { get; private set; }
    }
}