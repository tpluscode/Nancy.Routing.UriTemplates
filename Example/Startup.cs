using Example;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Nancy.Owin;

namespace Example
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseOwin(b => b.UseNancy(options => options.Bootstrapper = new Bootstrapper()));
        }
    }
}
