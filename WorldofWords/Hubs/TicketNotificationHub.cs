using Microsoft.AspNet.SignalR;
using System.Linq;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using WorldOfWords.Domain.Services.IServices;
using WorldofWords.App_Start;
using WorldOfWords.Domain.Services;

namespace WorldofWords.Hubs
{
    [HubName("ticketNotification")]
    [TicketHubAuthorization]
    public class TicketNotificationHub : Hub
    {
        private IUserService _userService;
        private ICourseService _courseService;
        private ConnectedUsersContainer _userContainer;
        public TicketNotificationHub(IUserService userService, ICourseService courseService, ConnectedUsersContainer usersContainer)
        {
            _userService = userService;
            _courseService = courseService;
            _userContainer = usersContainer;
        }

        public override System.Threading.Tasks.Task OnConnected()
        {
            string id = Context.QueryString.Get("id");
            lock (_userContainer)
            {
                _userContainer.UserIds.Add(id);
            }
            
            var roles = Context.QueryString.Get("role");
            if (roles.Contains("Admin"))
            {
                Groups.Add(Context.ConnectionId, "Admins");

                if(roles.Contains("Student") )
                {
                    Clients.Caller.updateUnreadTicketCounterForUser();
                }
                if(roles.Contains("Teacher"))
                {
                    Clients.Caller.updateUnreadTicketCounterForUser();
                }
                return Clients.Caller.updateUnreadTicketCounterForAdmin();
            }
            return Clients.Caller.updateUnreadTicketCounterForUser();
        }

        public void RemoveFromGroups()
        {
            string id = Context.QueryString.Get("id");
            lock (_userContainer)
            {
                _userContainer.UserIds.Remove(id);
            }

            var roles = Context.QueryString.Get("role");
            if (roles.Contains("Admin"))
            {
                Groups.Remove(Context.ConnectionId, "Admins");
            }
        }

        // stopCalled: 
        // true, if stop was called on the client closing the connection gracefully;
        // false, if the client timed out. Timeouts can be caused by clients reconnecting to another SignalR server in scaleout.

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {          
            RemoveFromGroups();          
            return base.OnDisconnected(stopCalled);
        }

        public void NotifyAboutChangeTicketState(string ownerId, string subject, string reviewStatus)
        {
            UpdateTicketTable(ownerId);
            UpdateUnreadTicketCounterForUser(ownerId);
            UpdateUnreadTicketCounterForAdmin();
            Clients.User(ownerId).notifyAboutChangeTicketState(subject, reviewStatus);
        }

        public void UpdateTicketTable(string ownerId)
        {
            Clients.User(ownerId).updateTicketTable();
            Clients.Group("Admins").updateTicketTable();
        }

        public void NotifyAdminsAboutNewTicket(string subject, string ownerId)
        {
            UpdateTicketTable(ownerId);
            UpdateUnreadTicketCounterForAdmin();
            Clients.Group("Admins").notifyAboutNewTicket(subject);
        }

        public void NotifyAboutSharedWordSuites(string[] teachersToShareId)
        {
            foreach (var id in teachersToShareId)
            {
                Clients.User(id).notifyAboutSharedWordSuites();
            }
        }

        public void UpdateUnreadTicketCounterForAdmin()
        {
             Clients.Group("Admins").updateUnreadTicketCounterForAdmin();
        }

        public void UpdateUnreadTicketCounterForUser(string ownerId)
        {
             Clients.User(ownerId).updateUnreadTicketCounterForUser();
        }

        public void NotifyAboutCourseChange(int courseId, string courseName)
        {
            // Get all users subscribed to the changed course
            IList<string> usersId = _courseService.GetUsersIdByCourseId(courseId);

            lock (_userContainer)
            {
                // Get list of subscribed and online users
                IList<string> connectedAndSubscribedUsers = _userContainer.UserIds.Intersect(usersId).Distinct().ToList();

                // sending notification
                Clients.Users(connectedAndSubscribedUsers).showMessageCourseChanged(courseName);
            }
        }
    }
}