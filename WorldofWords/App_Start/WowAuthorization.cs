using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using WorldOfWords.Domain.Services.MessagesAndConsts;

namespace WorldofWords
{
    sealed public class WowAuthorization : AuthorizeAttribute
    {
        private string[] _allRoles = new string[ConstContainer.MaxRoles];
        public string[] AllRoles 
        {
            get { return _allRoles; }
            set { _allRoles = value; } 
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            IPrincipal principal = Thread.CurrentPrincipal;
            return principal != null && principal.Identity != null && principal.Identity.IsAuthenticated;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (CheckHttpContext(actionContext))
            {
                if ((Thread.CurrentPrincipal.IsInRole(Roles)) || (IsInRoles(AllRoles)))
                {
                    return;
                }

                actionContext.Response = new HttpResponseMessage(HttpStatusCode.NoContent);
                return;
            }
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        private static bool IsInRoles(string[] roles)
        {
            if (roles != null)
            {
                return roles.Any(role => Thread.CurrentPrincipal.IsInRole(role));
            }

            return false;
        }

        private bool CheckHttpContext(HttpActionContext actionContext)
        {
            return ((actionContext != null) 
                && (actionContext.Request.Headers.Authorization != null)
                && (actionContext.Request.Headers.Authorization.Scheme != null)) 
                && IsAuthorized(actionContext);
        }
    }
}
