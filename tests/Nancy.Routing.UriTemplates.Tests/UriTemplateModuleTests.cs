using System.Linq;
using FluentAssertions;
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
        public void When_not_using_templates_Adds_ordinary_routes_when_using_routing_methods()
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
        public void Adds_template_route_for_GET()
        {
            // when
            using (this.module.Templates)
            {
                this.module.Get("/", (o, o2) => null);
            }

            // then
            this.module.TemplateRoutes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_template_route_for_POST()
        {
            // when
            using (this.module.Templates)
            {
                this.module.Post("/", (o, o2) => null);
            }

            // then
            this.module.TemplateRoutes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_template_route_for_OPTIONS()
        {
            // when
            using (this.module.Templates)
            {
                this.module.Options("/", (o, o2) => null);
            }

            // then
            this.module.TemplateRoutes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_template_route_for_PATCH()
        {
            // when
            using (this.module.Templates)
            {
                this.module.Patch("/", (o, o2) => null);
            }

            // then
            this.module.TemplateRoutes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_template_route_for_DELETE()
        {
            // when
            using (this.module.Templates)
            {
                this.module.Delete("/", (o, o2) => null);
            }

            // then
            this.module.TemplateRoutes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_template_route_for_HEAD()
        {
            // when
            using (this.module.Templates)
            {
                this.module.Head("/", (o, o2) => null);
            }

            // then
            this.module.TemplateRoutes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_template_route_for_PUT()
        {
            // when
            using (this.module.Templates)
            {
                this.module.Put("/", (o, o2) => null);
            }

            // then
            this.module.TemplateRoutes.Should().HaveCount(1);
        }

        [Theory]
        [InlineData("/static/path", "/static/path{?params*}")]
        [InlineData("/static/path{?some}", "/static/path{?some,params*}")]
        [InlineData("/static/path?required={required}", "/static/path?required={required}{&params*}")]
        [InlineData("/static/path{?some}{&more}", "/static/path{?some}{&more,params*}")]
        [InlineData("/static/path{/more,path}", "/static/path{/more,path}{?params*}")]
        [InlineData("/static/path/{more,path}", "/static/path/{more,path}{?params*}")]
        [InlineData("/static/path/{path,explode*}", "/static/path/{path,explode*}{?params*}")]
        [InlineData("/static/path{/path,explode*}", "/static/path{/path,explode*}{?params*}")]
        [InlineData("/static/path{/path}{;path_params}", "/static/path{/path}{;path_params}{?params*}")]
        [InlineData("/static/path{/path}{;path_params*}", "/static/path{/path}{;path_params*}{?params*}")]
        public void Adds_template_path_with_appended_query_string_wildcard(
            string invokedPath,
            string actualPath)
        {
            // when
            using (this.module.Templates)
            {
                this.module.Get(invokedPath, (o, o2) => null);
            }

            // then
            this.module.TemplateRoutes.Single().Description.Path.Should().Be(actualPath);
        }

        [Fact]
        public void Adds_template_as_is_in_strict_mode()
        {
            // given
            const string path = "/static/path{?some}";

            // when
            using (this.module.Templates.Strict)
            {
                this.module.Get(path, (o, o2) => null);
            }

            // then
            this.module.TemplateRoutes.Single().Description.Path.Should().Be(path);
        }

        [Fact]
        public void Adds_oridinary_route_when_Templates_is_disposed()
        {
            // when
            using (this.module.Templates)
            {
            }

            this.module.Get("/", (o, o2) => null);

            // then
            this.module.Routes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_oridinary_route_when_Strict_Templates_is_disposed()
        {
            // when
            using (this.module.Templates.Strict)
            {
            }

            this.module.Get("/", (o, o2) => null);

            // then
            this.module.Routes.Should().HaveCount(1);
        }

        [Theory]
        [InlineData("/static/path{?exists*}")]
        [InlineData("/static/path{?some}{&exists*}")]
        public void Adds_template_path_as_is_When_query_string_wildcard_already_exists(string path)
        {
            // when
            using (this.module.Templates)
            {
                this.module.Get(path, (o, o2) => null);
            }

            // then
            this.module.TemplateRoutes.Single().Description.Path.Should().Be(path);
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
