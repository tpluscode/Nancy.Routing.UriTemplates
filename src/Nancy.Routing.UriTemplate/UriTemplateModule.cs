using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nancy.Routing.UriTemplate
{
    public abstract class UriTemplateModule : NancyModule, IUriTemplateRouting
    {
        private readonly IList<TemplateRoute> templateRoutes = new List<TemplateRoute>();

        protected UriTemplateModule()
        {
        }

        protected UriTemplateModule(string modulePath)
            : base(modulePath)
        {
        }

        public IEnumerable<TemplateRoute> TemplateRoutes => this.templateRoutes;

        public virtual void GetByTemplate(string template, Func<dynamic, object> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.GetByTemplate<object>(template, action, condition, name);
        }

        public virtual void GetByTemplate<T>(string template, Func<dynamic, T> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.GetByTemplate(template, args => Task.FromResult(action((DynamicDictionary)args)), condition, name);
        }

        public virtual void GetByTemplate<T>(string template, Func<dynamic, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.GetByTemplate(template, (args, ct) => action((DynamicDictionary)args), condition, name);
        }

        public virtual void GetByTemplate<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.AddTemplateRoute("GET", this.GetAbsoluteTemplate(template), action, condition, name);
        }

        protected void AddTemplateRoute<T>(string method, string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition, string name)
        {
            this.templateRoutes.Add(new TemplateRoute<T>(name ?? string.Empty, method, template, condition, action));
        }

        private string GetAbsoluteTemplate(string template)
        {
            var relativePath = (template ?? string.Empty).Trim('/');
            var parentPath = (this.ModulePath ?? string.Empty).Trim('/');

            if (string.IsNullOrEmpty(parentPath))
            {
                return string.Concat("/", relativePath);
            }

            if (string.IsNullOrEmpty(relativePath))
            {
                return string.Concat("/", parentPath);
            }

            var separator = "/";
            if (relativePath.StartsWith("{/"))
            {
                separator = string.Empty;
            }

            return string.Concat("/", parentPath, separator, relativePath);
        }
    }
}