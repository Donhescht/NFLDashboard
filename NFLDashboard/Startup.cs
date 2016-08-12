using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NFLDashboard.Startup))]
namespace NFLDashboard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
