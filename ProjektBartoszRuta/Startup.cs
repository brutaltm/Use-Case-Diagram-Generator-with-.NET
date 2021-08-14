using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjektBartoszRuta.Startup))]
namespace ProjektBartoszRuta
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
