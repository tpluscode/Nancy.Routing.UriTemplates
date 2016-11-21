using Nancy;
using Nancy.Routing.UriTemplate;

namespace Example
{
    public class TestModule : UriTemplateModule
    {
        public TestModule() : base("/base/path/")
        {
            using (Templates)
            {
                Get("{/seg1}/{.seg2,seg3}{/page}{?filter}", _ =>
                {
                    return Response.AsJson(new
                    {
                        _.seg1,
                        _.seg2,
                        _.seg3,
                        _.page,
                        _.filter
                    });
                });
                Get("/literal/{.seg2,seg3}{/page}{?filter}", _ =>
                {
                    return Response.AsJson(new
                    {
                        _.articles,
                        _.seg2,
                        _.seg3,
                        _.page,
                        _.filter
                    });
                });
            }
        }
    }
}