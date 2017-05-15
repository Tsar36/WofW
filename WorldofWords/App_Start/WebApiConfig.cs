using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Elmah.Contrib.WebApi;
using Microsoft.Owin.Security.OAuth;
using WorldofWords.ExceptionLogging;
using WorldOfWords.Domain.Services.Services;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WorldofWords.Filters;

namespace WorldofWords
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Filters.Add(new ModelValidationFilterAttribute());
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Re‌​ferenceLoopHandling = ReferenceLoopHandling.Ignore;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // There can be multiple exception loggers. (By default, no exception loggers are registered.)
            config.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());
            config.MessageHandlers.Add(new HttpMessageHandler(new RequestIdentityService(new UnitOfWorkFactory())));
        }
    }
}
