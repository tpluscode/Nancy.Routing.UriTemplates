﻿using System;
using System.Linq;
using Nancy.Configuration;

namespace Nancy.Routing.UriTemplate
{
    public class UriTemplateRouteResolver : IRouteResolver
    {
        private readonly IRouteResolver inner;
        private readonly INancyModuleCatalog moduleCatalog;
        private readonly INancyModuleBuilder builder;
        private readonly GlobalizationConfiguration globalizationConfiguraton;

        public UriTemplateRouteResolver(DefaultRouteResolver inner, INancyModuleCatalog moduleCatalog, INancyModuleBuilder builder, INancyEnvironment environment)
        {
            this.inner = inner;
            this.moduleCatalog = moduleCatalog;
            this.builder = builder;
            this.globalizationConfiguraton = environment.GetValue<GlobalizationConfiguration>();
        }

        public ResolveResult Resolve(NancyContext context)
        {
            var uri = new Uri(Uri.EscapeUriString(context.Request.Path + context.Request.Url.Query), UriKind.Relative);
            var matches = (from module in this.moduleCatalog.GetAllModules(context).OfType<IUriTemplateRouting>()
                           from route in module.TemplateRoutes
                           where route.Description.Method.Equals(context.Request.Method, StringComparison.OrdinalIgnoreCase)
                           let match = new TunnelVisionLabs.Net.UriTemplate(route.Description.Path).Match(uri)
                           where match != null
                           select new
                           {
                               module,
                               route,
                               match
                           }).ToList();

            if (matches.Any())
            {
                var match = matches.Single();
                if (match.match != null)
                {
                    if (match.route.Description.Condition == null || match.route.Description.Condition.Invoke(context))
                    {
                        context.NegotiationContext.SetModule(this.builder.BuildModule(match.module, context));
                        var paramDict = match.match.Bindings.ToDictionary(pair => pair.Key, pair => pair.Value.Value);
                        var parameters = DynamicDictionary.Create(paramDict, this.globalizationConfiguraton);

                        return new ResolveResult
                        {
                            Route = match.route,
                            Parameters = parameters,
                            After = match.module.After,
                            Before = match.module.Before,
                            OnError = match.module.OnError
                        };
                    }
                }
            }

            var resolveResult = this.inner.Resolve(context);
            return resolveResult;
        }
    }
}