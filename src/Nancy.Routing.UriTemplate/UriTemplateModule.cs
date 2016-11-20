using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nancy.Routing.UriTemplate
{
    public abstract class UriTemplateModule : NancyModule, IUriTemplateRouting
    {
        private readonly IList<Route> templateRoutes = new List<Route>();

        protected UriTemplateModule()
        {
        }

        protected UriTemplateModule(string modulePath)
            : base(modulePath)
        {
        }

        public IEnumerable<Route> TemplateRoutes => this.templateRoutes;

        public override void Get(string path, Func<dynamic, object> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Get<object>(path, action, condition, name);
        }

        public override void Get<T>(string path, Func<dynamic, T> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Get(path, args => Task.FromResult(action((DynamicDictionary)args)), condition, name);
        }

        public override void Get<T>(string path, Func<dynamic, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Get(path, (args, ct) => action((DynamicDictionary)args), condition, name);
        }

        public override void Get<T>(string path, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.AddRoute("GET", new TunnelVisionLabs.Net.UriTemplate(this.GetFullPath(path)), action, condition, name);
        }

        protected void AddRoute<T>(string method, TunnelVisionLabs.Net.UriTemplate template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition, string name)
        {
            this.templateRoutes.Add(new Route<T>(name ?? string.Empty, method, template.ToString(), condition, action));
        }

        private string GetFullPath(string path)
        {
            var relativePath = (path ?? string.Empty).Trim('/');
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