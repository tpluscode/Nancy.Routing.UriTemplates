using System;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.Routing.UriTemplate;

namespace Example
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        public override void Configure(INancyEnvironment environment)
        {
            base.Configure(environment);
            environment.Tracing(enabled: false, displayErrorTraces: true);
        }

        protected override Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration
        {
            get
            {
                return catalog =>
                {
                    var configuration = base.InternalConfiguration(catalog);
                    configuration.RouteResolver = typeof(UriTemplateRouteResolver);
                    return configuration;
                };
            }
        }
    }
}