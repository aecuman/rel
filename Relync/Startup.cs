using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Relync.Startup))]
namespace Relync
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
