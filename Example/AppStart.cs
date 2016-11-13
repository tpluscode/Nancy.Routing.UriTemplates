using Example;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AppStart))]

namespace Example
{
    public class AppStart
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy(options => options.Bootstrapper = new Bootstrapper());
        }
    }
}