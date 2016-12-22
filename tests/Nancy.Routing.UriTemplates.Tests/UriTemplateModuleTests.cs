using System.Linq;
using FluentAssertions;
using Xunit;

namespace Nancy.Routing.UriTemplates.Tests
{
    public class UriTemplateModuleTests
    {
        [Fact]
        public void When_not_using_templates_Adds_ordinary_routes_when_using_routing_methods()
        {
            // when
            UriTemplateModule module1 = new UriTemplateModuleTestable();
            module1.Get("/", (o, o2) => null);
            module1.Put("/", (o, o2) => null);
            module1.Post("/", (o, o2) => null);
            module1.Patch("/", (o, o2) => null);
            module1.Delete("/", (o, o2) => null);
            module1.Head("/", (o, o2) => null);
            module1.Options("/", (o, o2) => null);

            // then
            module1.Routes.Should().HaveCount(7);
        }

        [Fact]
        public void Adds_template_route_for_GET()
        {
            // when
            UriTemplateModule module1 = new UriTemplateModuleTestable();
            using (module1.Templates)
            {
                module1.Get("/", (o, o2) => null);
            }

            // then
            module1.TemplateRoutes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_template_route_for_POST()
        {
            // when
            UriTemplateModule module1 = new UriTemplateModuleTestable();
            using (module1.Templates)
            {
                module1.Post("/", (o, o2) => null);
            }

            // then
            module1.TemplateRoutes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_template_route_for_OPTIONS()
        {
            // when
            UriTemplateModule module1 = new UriTemplateModuleTestable();
            using (module1.Templates)
            {
                module1.Options("/", (o, o2) => null);
            }

            // then
            module1.TemplateRoutes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_template_route_for_PATCH()
        {
            // when
            UriTemplateModule module1 = new UriTemplateModuleTestable();
            using (module1.Templates)
            {
                module1.Patch("/", (o, o2) => null);
            }

            // then
            module1.TemplateRoutes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_template_route_for_DELETE()
        {
            // when
            UriTemplateModule module1 = new UriTemplateModuleTestable();
            using (module1.Templates)
            {
                module1.Delete("/", (o, o2) => null);
            }

            // then
            module1.TemplateRoutes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_template_route_for_HEAD()
        {
            // when
            UriTemplateModule module1 = new UriTemplateModuleTestable();
            using (module1.Templates)
            {
                module1.Head("/", (o, o2) => null);
            }

            // then
            module1.TemplateRoutes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_template_route_for_PUT()
        {
            // when
            UriTemplateModule module1 = new UriTemplateModuleTestable();
            using (module1.Templates)
            {
                module1.Put("/", (o, o2) => null);
            }

            // then
            module1.TemplateRoutes.Should().HaveCount(1);
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
            UriTemplateModule module1 = new UriTemplateModuleTestable();
            using (module1.Templates)
            {
                module1.Get(invokedPath, (o, o2) => null);
            }

            // then
            module1.TemplateRoutes.Single().Description.Path.Should().Be(actualPath);
        }

        [Fact]
        public void Adds_template_as_is_in_strict_mode()
        {
            // given
            const string path = "/static/path{?some}";

            // when
            UriTemplateModule module1 = new UriTemplateModuleTestable();
            using (module1.Templates.Strict)
            {
                module1.Get(path, (o, o2) => null);
            }

            // then
            module1.TemplateRoutes.Single().Description.Path.Should().Be(path);
        }

        [Fact]
        public void Adds_oridinary_route_when_Templates_is_disposed()
        {
            // when
            UriTemplateModule module1 = new UriTemplateModuleTestable();
            using (module1.Templates)
            {
            }

            module1.Get("/", (o, o2) => null);

            // then
            module1.Routes.Should().HaveCount(1);
        }

        [Fact]
        public void Adds_oridinary_route_when_Strict_Templates_is_disposed()
        {
            // when
            UriTemplateModule module1 = new UriTemplateModuleTestable();
            using (module1.Templates.Strict)
            {
            }

            module1.Get("/", (o, o2) => null);

            // then
            module1.Routes.Should().HaveCount(1);
        }

        [Theory]
        [InlineData("/static/path{?exists*}")]
        [InlineData("/static/path{?some}{&exists*}")]
        public void Adds_template_path_as_is_When_query_string_wildcard_already_exists(string path)
        {
            // when
            UriTemplateModule module1 = new UriTemplateModuleTestable();
            using (module1.Templates)
            {
                module1.Get(path, (o, o2) => null);
            }

            // then
            module1.TemplateRoutes.Single().Description.Path.Should().Be(path);
        }

        [Theory]
        [InlineData("/base", "{route}", "/base/{route}")]
        [InlineData("/base", "/{route}", "/base/{route}")]
        [InlineData("/base", "{/route}", "/base{/route}")]
        [InlineData("/base/", "{route}", "/base/{route}")]
        [InlineData("/base/", "/{route}", "/base/{route}")]
        [InlineData("/base/", "{/route}", "/base{/route}")]
        public void Adds_template_concantenated_with_base_path(string basePath, string template, string route)
        {
            // given
            var module = new UriTemplateModuleTestable(basePath);

            // when
            using (module.Templates.Strict)
            {
                module.Get(template, (o, token) => null);
            }

            // then
            module.TemplateRoutes.Single().Description.Path.Should().Be(route);
        }

        [Theory]
        [InlineData("base", "route", "/base/route")]
        [InlineData("", "route", "/route")]
        [InlineData(null, "route", "/route")]
        [InlineData(null, "{/route}", "{/route}")]
        public void Adds_template_always_beginning_with_slash(string basePath, string template, string route)
        {
            // given
            var module = new UriTemplateModuleTestable(basePath);

            // when
            using (module.Templates.Strict)
            {
                module.Get(template, (o, token) => null);
            }

            // then
            module.TemplateRoutes.Single().Description.Path.Should().Be(route);
        }

        [Theory]
        [InlineData("{/base}", "route", "{/base}/route")]
        [InlineData("", "{/route}", "{/route}")]
        [InlineData(null, "{/route}", "{/route}")]
        public void Does_not_prepend_slash_to_template_whuch_begins_with_segment_expression(string basePath, string template, string route)
        {
            // given
            var module = new UriTemplateModuleTestable(basePath);

            // when
            using (module.Templates.Strict)
            {
                module.Get(template, (o, token) => null);
            }

            // then
            module.TemplateRoutes.Single().Description.Path.Should().Be(route);
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
