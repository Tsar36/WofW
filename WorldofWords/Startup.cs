using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web.Routing;
using Unity.WebApi;
using WorldofWords;
using WorldofWords.App_Start;

[assembly: OwinStartup(typeof(Startup))]

namespace WorldofWords
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.DependencyResolver = new SignalRUnityDependencyResolver(UnityConfig.GetConfiguredContainer());

            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new UserIdProvider());
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            app.UseWebApi(RunWebApiConfiguration());
            app.MapSignalR("/signalr", new Microsoft.AspNet.SignalR.HubConfiguration()
            {
                EnableJavaScriptProxies = false
            });
        }

        private HttpConfiguration RunWebApiConfiguration()
        {
            HttpConfiguration config = new HttpConfiguration();
            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());
            WebApiConfig.Register(config);
            return config;
        }
    }
}
