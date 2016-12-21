using System.Threading.Tasks;
using FluentAssertions;
using Nancy.Routing.UriTemplates.Tests.Functional.Modules;
using Nancy.Testing;
using Xunit;

namespace Nancy.Routing.UriTemplates.Tests.Functional
{
    public class UriTemplateRouteResolverFixture
    {
        [Theory]
        [InlineData("base")]
        [InlineData("base/")]
        [InlineData("/base")]
        [InlineData("/base/")]
        public async Task Should_return_from_module_with_base(string basePath)
        {
            // given
            INancyModule moduleWithBase = new ModuleWithBase(basePath);
            Browser browser = new Browser(with => with.Module(moduleWithBase).RouteResolver<UriTemplateRouteResolver>());

            // when
            var response = await browser.Get("base/hello/path");

            // then
            Assert.Equal("hello", response.Body.DeserializeJson<dynamic>().optional);
        }
    }
}
