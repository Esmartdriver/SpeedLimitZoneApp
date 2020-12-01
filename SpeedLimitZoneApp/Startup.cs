using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SpeedLimitZoneApp.Startup))]
namespace SpeedLimitZoneApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
