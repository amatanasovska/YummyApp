using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(YummyApp.Startup))]
namespace YummyApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
