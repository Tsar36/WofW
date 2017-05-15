using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorldofWords.Hubs
{
    public class ConnectedUsersContainer
    {
        private static ConnectedUsersContainer connectedUsersContainer = null;
        public List<string> UserIds
        {
            get;
            set;
        }

        public static ConnectedUsersContainer Container
        {
            get
            {
                return connectedUsersContainer ?? (connectedUsersContainer = new ConnectedUsersContainer());
            }
        }

        private ConnectedUsersContainer()
        {
            UserIds = new List<string>();
        }
    }
}