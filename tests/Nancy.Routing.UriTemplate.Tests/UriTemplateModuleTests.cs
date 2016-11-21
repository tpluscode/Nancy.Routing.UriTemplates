using System;
using FluentAssertions;
using Nancy.Routing.UriTemplate;
using Xunit;

namespace Nancy.Routing.UriTemplates.Tests
{
    public class UriTemplateModuleTests
    {
        private readonly UriTemplateModule module;

        public UriTemplateModuleTests()
        {
            this.module = new UriTemplateModuleTestable();
        }

        [Fact]
        public void Adds_ordinary_routes_when_using_routing_methods()
        {
            // when
            this.module.Get("/", (o, o2) => null);
            this.module.Put("/", (o, o2) => null);
            this.module.Post("/", (o, o2) => null);
            this.module.Patch("/", (o, o2) => null);
            this.module.Delete("/", (o, o2) => null);
            this.module.Head("/", (o, o2) => null);
            this.module.Options("/", (o, o2) => null);

            // then
            this.module.Routes.Should().HaveCount(7);
        }

        [Fact]
        public void When_using_templates_Adds_template_route_for_GET()
        {
            // when
            using (this.module.Templates)
            {
                this.module.Get("/", (o, o2) => null);
            }

            // then
            this.module.TemplateRoutes.Should().HaveCount(1);
        }

        public class UriTemplateModuleTestable : UriTemplateModule
        {
            public UriTemplateModuleTestable()
            {
            }

            public UriTemplateModuleTestable(string modulePath)
                : base(modulePath)
            {
            }
        }
    }
}
