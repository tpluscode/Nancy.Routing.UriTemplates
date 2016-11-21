using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nancy.Routing.UriTemplate
{
    public abstract class UriTemplateModule : NancyModule, IUriTemplateRouting
    {
        private readonly IList<TemplateRoute> templateRoutes = new List<TemplateRoute>();

        private bool isUsingTemplates;

        protected UriTemplateModule()
        {
        }

        protected UriTemplateModule(string modulePath)
            : base(modulePath)
        {
        }

        public IEnumerable<TemplateRoute> TemplateRoutes => this.templateRoutes;

        public IDisposable Templates => new UsingTemplatesToggle(this);

        public override void Get<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.isUsingTemplates)
            {
                this.AddTemplateRoute("GET", this.GetAbsoluteTemplate(template), action, condition, name);
            }
            else
            {
                this.AddRoute("GET", this.GetAbsoluteTemplate(template), action, condition, name);
            }
        }

        public override void Put<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.isUsingTemplates)
            {
                this.AddTemplateRoute("PUT", this.GetAbsoluteTemplate(template), action, condition, name);
            }
            else
            {
                this.AddRoute("PUT", this.GetAbsoluteTemplate(template), action, condition, name);
            }
        }

        public override void Post<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.isUsingTemplates)
            {
                this.AddTemplateRoute("POST", this.GetAbsoluteTemplate(template), action, condition, name);
            }
            else
            {
                this.AddRoute("POST", this.GetAbsoluteTemplate(template), action, condition, name);
            }
        }

        public override void Patch<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.isUsingTemplates)
            {
                this.AddTemplateRoute("PATCH", this.GetAbsoluteTemplate(template), action, condition, name);
            }
            else
            {
                this.AddRoute("PATCH", this.GetAbsoluteTemplate(template), action, condition, name);
            }
        }

        public override void Head<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.isUsingTemplates)
            {
                this.AddTemplateRoute("HEAD", this.GetAbsoluteTemplate(template), action, condition, name);
            }
            else
            {
                this.AddRoute("HEAD", this.GetAbsoluteTemplate(template), action, condition, name);
            }
        }

        public override void Delete<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.isUsingTemplates)
            {
                this.AddTemplateRoute("DELETE", this.GetAbsoluteTemplate(template), action, condition, name);
            }
            else
            {
                this.AddRoute("DELETE", this.GetAbsoluteTemplate(template), action, condition, name);
            }
        }

        public override void Options<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.isUsingTemplates)
            {
                this.AddTemplateRoute("OPTIONS", this.GetAbsoluteTemplate(template), action, condition, name);
            }
            else
            {
                this.AddRoute("OPTIONS", this.GetAbsoluteTemplate(template), action, condition, name);
            }
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

        private class UsingTemplatesToggle : IDisposable
        {
            private readonly UriTemplateModule uriTemplateModule;

            public UsingTemplatesToggle(UriTemplateModule uriTemplateModule)
            {
                uriTemplateModule.isUsingTemplates = true;
                this.uriTemplateModule = uriTemplateModule;
            }

            public void Dispose()
            {
                this.uriTemplateModule.isUsingTemplates = false;
            }
        }
    }
}