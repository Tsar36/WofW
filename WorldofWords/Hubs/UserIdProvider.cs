using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorldofWords.App_Start
{
    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException("request");
            }
            if(request.User != null && request.User.Identity != null)
            {
                return request.QueryString.Get("id");
            }
            return (string)null;
        }
    }
}