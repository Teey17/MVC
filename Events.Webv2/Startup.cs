using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Events.Webv2.Startup))]
namespace Events.Webv2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
