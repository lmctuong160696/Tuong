using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProWeb.Startup))]
namespace ProWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
