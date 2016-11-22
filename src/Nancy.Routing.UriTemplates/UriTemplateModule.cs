using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Nancy.Routing.UriTemplates
{
    public abstract class UriTemplateModule : NancyModule, IUriTemplateRouting
    {
        private static readonly Regex LastTemplateExpressionRegex = new Regex(@"(?<before>.)?(?<lastExpression>{(?<op>[?&;]?)(?<var>[\w\d_%,]+(?<explode>\*?))})$");

        private readonly IList<TemplateRoute> templateRoutes = new List<TemplateRoute>();

        private RoutingMode routingMode;

        protected UriTemplateModule()
        {
        }

        protected UriTemplateModule(string modulePath)
            : base(modulePath)
        {
        }

        public IEnumerable<TemplateRoute> TemplateRoutes => this.templateRoutes;

        public IEnableTemplateRouting Templates => new UsingTemplatesToggle(this);

        public override void Get<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.routingMode.HasFlag(RoutingMode.Templates))
            {
                this.AddTemplateRoute("GET", template, action, condition, name);
            }
            else
            {
                base.Get(template, action, condition, name);
            }
        }

        public override void Put<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.routingMode.HasFlag(RoutingMode.Templates))
            {
                this.AddTemplateRoute("PUT", template, action, condition, name);
            }
            else
            {
                base.Put(template, action, condition, name);
            }
        }

        public override void Post<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.routingMode.HasFlag(RoutingMode.Templates))
            {
                this.AddTemplateRoute("POST", template, action, condition, name);
            }
            else
            {
                base.Post(template, action, condition, name);
            }
        }

        public override void Patch<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.routingMode.HasFlag(RoutingMode.Templates))
            {
                this.AddTemplateRoute("PATCH", template, action, condition, name);
            }
            else
            {
                base.Patch(template, action, condition, name);
            }
        }

        public override void Head<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.routingMode.HasFlag(RoutingMode.Templates))
            {
                this.AddTemplateRoute("HEAD", template, action, condition, name);
            }
            else
            {
                base.Head(template, action, condition, name);
            }
        }

        public override void Delete<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.routingMode.HasFlag(RoutingMode.Templates))
            {
                this.AddTemplateRoute("DELETE", template, action, condition, name);
            }
            else
            {
                base.Delete(template, action, condition, name);
            }
        }

        public override void Options<T>(string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            if (this.routingMode.HasFlag(RoutingMode.Templates))
            {
                this.AddTemplateRoute("OPTIONS", template, action, condition, name);
            }
            else
            {
                base.Options(template, action, condition, name);
            }
        }

        protected void AddTemplateRoute<T>(string method, string template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition, string name)
        {
            if (this.routingMode.HasFlag(RoutingMode.Strict) == false)
            {
                template = EnsureGreedyQueryString(template);
            }

            var absoluteTemplate = this.GetAbsoluteTemplate(template);
            this.templateRoutes.Add(new TemplateRoute<T>(name ?? string.Empty, method, absoluteTemplate, condition, action));
        }

        private static string EnsureGreedyQueryString(string template)
        {
            var lastExprMatch = LastTemplateExpressionRegex.Match(template);

            if (lastExprMatch.Success == false)
            {
                return template + "{?params*}";
            }

            if (lastExprMatch.Groups["op"].Value == string.Empty)
            {
                if (lastExprMatch.Groups["before"].Value == "/")
                {
                    return template + "{?params*}";
                }

                return template + "{&params*}";
            }

            if (lastExprMatch.Groups["explode"].Value == "*")
            {
                if (lastExprMatch.Groups["op"].Value == "?" || lastExprMatch.Groups["op"].Value == "&")
                {
                    return template;
                }

                return template + "{?params*}";
            }

            if (lastExprMatch.Groups["op"].Value == "?" || lastExprMatch.Groups["op"].Value == "&")
            {
                return LastTemplateExpressionRegex.Replace(template, "$1{$3$4,params*}");
            }

            return template + "{?params*}";
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

        private class UsingTemplatesToggle : IEnableTemplateRouting
        {
            private readonly UriTemplateModule uriTemplateModule;

            public UsingTemplatesToggle(UriTemplateModule uriTemplateModule)
            {
                uriTemplateModule.routingMode = RoutingMode.Templates;
                this.uriTemplateModule = uriTemplateModule;
            }

            public IDisposable Strict
            {
                get
                {
                    this.uriTemplateModule.routingMode |= RoutingMode.Strict;
                    return this;
                }
            }

            public void Dispose()
            {
                this.uriTemplateModule.routingMode = RoutingMode.Standard;
            }
        }
    }
}