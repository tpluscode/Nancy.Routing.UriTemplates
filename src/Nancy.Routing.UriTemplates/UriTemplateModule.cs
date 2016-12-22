using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UriTemplateString.Spec;

namespace Nancy.Routing.UriTemplates
{
    public abstract class UriTemplateModule : NancyModule, IUriTemplateRouting
    {
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

        protected void AddTemplateRoute<T>(string method, UriTemplateString.UriTemplateString template, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition, string name)
        {
            if (this.routingMode.HasFlag(RoutingMode.Strict) == false)
            {
                var lastPart = template.Parts.Last();

                if (lastPart is Expression)
                {
                    var expression = (Expression)lastPart;
                    var isQuery = expression.Operator.Equals(Operator.QueryComponent) ||
                                  expression.Operator.Equals(Operator.QueryContinuation);
                    var isExploded = expression.VariableList.Last().Modifier is Explode;

                    if (isQuery == false || isExploded == false)
                    {
                        template = template.AppendQueryParam("params", true);
                    }
                }
                else
                {
                    template = template.AppendQueryParam("params", true);
                }
            }

            var absoluteTemplate = this.GetAbsoluteTemplate(template);
            this.templateRoutes.Add(new TemplateRoute<T>(name ?? string.Empty, method, absoluteTemplate, condition, action));
        }

        private string GetAbsoluteTemplate(UriTemplateString.UriTemplateString template)
        {
            var parentPath = (this.ModulePath ?? string.Empty).Trim('/');

            if (template.StartsWithSlash() == false)
            {
                parentPath += "/";
            }

            if (string.IsNullOrWhiteSpace(parentPath))
            {
                return template.ToString();
            }

            UriTemplateString.UriTemplateString parentTemplate = parentPath;

            if (parentTemplate.StartsWithSlash() == false)
            {
                parentTemplate = "/" + parentTemplate;
            }

            return (parentTemplate + template).ToString();
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