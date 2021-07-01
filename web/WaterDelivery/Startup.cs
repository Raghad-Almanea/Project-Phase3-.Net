using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WaterDelivery.Startup))]
namespace WaterDelivery
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
