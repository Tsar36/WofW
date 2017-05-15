using System;
using System.Security.Principal;
using System.Threading;
using System.Web.Http;
using WorldOfWords.Domain.Services.MessagesAndConsts;

namespace WorldofWords.Controllers
{
    public class BaseController : ApiController
    {        
        public BaseController()
        {
            UserId = GetUserId();
        }

        public int UserId { get; private set; }

        private int GetUserId()
        {
            IPrincipal principal = Thread.CurrentPrincipal;
            if (!string.IsNullOrEmpty(principal.Identity.Name))
            {
                return  Convert.ToInt32(principal.Identity.Name);
            }
            return ConstContainer.BadId;
        }
    }
}