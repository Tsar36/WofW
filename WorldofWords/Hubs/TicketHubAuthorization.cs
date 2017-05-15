using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Owin;
using Microsoft.Practices.Unity;
using System.Threading;
using WorldofWords.App_Start;
using WorldOfWords.Domain.Services.IServices;

namespace WorldofWords.Hubs
{
    public class TicketHubAuthorization : AuthorizeAttribute
    {
        public IRequestIdentityService Service
        {
            get
            {
                return UnityConfig.GetConfiguredContainer().Resolve<IRequestIdentityService>();
            }
        }

        public override bool AuthorizeHubConnection(Microsoft.AspNet.SignalR.Hubs.HubDescriptor hubDescriptor, IRequest request)
        {
            string id = request.QueryString.Get("id");
            string hashedToken = request.QueryString.Get("hashToken");
            return Service.CheckIdentity(hashedToken, id);
        }

        public override bool AuthorizeHubMethodInvocation(Microsoft.AspNet.SignalR.Hubs.IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {
            var connectionId = hubIncomingInvokerContext.Hub.Context.ConnectionId;
            var environment = hubIncomingInvokerContext.Hub.Context.Request.Environment;
            string hashToken = hubIncomingInvokerContext.Hub.Context.Request.QueryString.Get("hashToken");
            string id = hubIncomingInvokerContext.Hub.Context.Request.QueryString.Get("id");
            bool isAuthorized = Service.CheckIdentity(hashToken, id);
            if (isAuthorized)
            {
                hubIncomingInvokerContext.Hub.Context = new HubCallerContext(new ServerRequest(environment), connectionId);
                return true;
            }
            return false;
        }
    }
}