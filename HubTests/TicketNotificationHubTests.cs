using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit;
using Microsoft.AspNet.SignalR;
using WorldofWords.Hubs;
using NUnit.Framework;
using Microsoft.AspNet.SignalR.Hubs;
using System.Dynamic;
using Microsoft.AspNet.SignalR.Hosting;
using WorldOfWords.Domain.Services;
using WorldOfWords.Domain.Services.IServices;

namespace HubTests
{
    [TestFixture]
    public class TicketNotificationHubTests
    {
        TicketNotificationHub hub;
        Mock<IHubCallerConnectionContext<dynamic>> mockClients;
        Mock<IGroupManager> mockGroups;
        Mock<IUserService> userService;
        Mock<ICourseService> courseService;
        ConnectedUsersContainer usersContainer;

        dynamic someUser;
        dynamic someGroup;

        [SetUp]
        public void SetUp()
        {
            userService = new Mock<IUserService>();
            courseService = new Mock<ICourseService>();
            usersContainer = ConnectedUsersContainer.Container;
            hub = new TicketNotificationHub(userService.Object, courseService.Object, usersContainer);
            someUser = new ExpandoObject();
            someGroup = new ExpandoObject();
            mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
            mockGroups = new Mock<IGroupManager>();
            hub.Clients = mockClients.Object;
            hub.Groups = mockGroups.Object;
            mockClients.Setup(mc => mc.User(It.IsAny<string>())).Returns((ExpandoObject)someUser);
            mockClients.Setup(mc => mc.Group("Admins")).Returns((ExpandoObject)someGroup);
        }

        [Test]
        public void UpdateUnreadTicketCounterForUserTest()
        {
            //Arrange
            bool updateUnreadTicketCounterForUser = false;            
            someUser.updateUnreadTicketCounterForUser = new Action(() =>
            {
                updateUnreadTicketCounterForUser = true;
            });

            //Act
            hub.UpdateUnreadTicketCounterForUser(It.IsAny<string>());
            
            //Assert
            mockClients.Verify(mc => mc.User(It.IsAny<string>()), Times.Once);
            Assert.IsTrue(updateUnreadTicketCounterForUser);
        }

        [Test]
        public void UpdateUnreadTicketCounterForAdmin()
        {
            //Arrange
            bool updateUnreadTicketCounterForAdmin = false;
            someGroup.updateUnreadTicketCounterForAdmin = new Action(() =>
            {
                updateUnreadTicketCounterForAdmin = true;
            });

            //Act
            hub.UpdateUnreadTicketCounterForAdmin();

            //Assert
            mockClients.Verify(mc => mc.Group(It.IsAny<string>()), Times.Once);
            Assert.IsTrue(updateUnreadTicketCounterForAdmin);
        }

        [Test]
        public void UpdateTicketTableTest()
        {
            //Arrange
            bool updateTicketTableUser = false;
            bool updateTicketTableAdmin = false;
            someUser.updateTicketTable = new Action(() =>
            {
                updateTicketTableUser = true;
            });
            someGroup.updateTicketTable = new Action(() =>
            {
                updateTicketTableAdmin = true;
            });

            //Act
            hub.UpdateTicketTable(It.IsAny<string>());

            //Assert
            mockClients.Verify(mg => mg.Group(It.IsAny<string>()), Times.Once);
            mockClients.Verify(mc => mc.User(It.IsAny<string>()), Times.Once);
            Assert.IsTrue(updateTicketTableUser && updateTicketTableAdmin);
        }

        [Test]
        public void NotifyAboutChangeTicketStateTest()
        {
            //Arrange
            bool updateUserTable = false;
            bool updateAdminTable = false;
            bool updateUnreadTicketCounterUser = false;
            bool updateUnreadTicketCounterAdmin = false;
            bool notifyAboutChangeTicketState = false;
            someUser.updateTicketTable = new Action(() =>
            {
                updateUserTable = true;
            });
            someGroup.updateTicketTable = new Action(() =>
            {
                updateAdminTable = true;
            });
            someGroup.updateUnreadTicketCounterForAdmin = new Action(() =>
            {
                updateUnreadTicketCounterAdmin = true;
            });
            someUser.updateUnreadTicketCounterForUser = new Action(() =>
            {
                updateUnreadTicketCounterUser = true;
            });
            someUser.notifyAboutChangeTicketState = new Action<string, string>((subject, reviewStatus) =>
            {
                notifyAboutChangeTicketState = true;
            });

            //Act
            hub.NotifyAboutChangeTicketState(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            //Assert
            mockClients.Verify(mc => mc.User(It.IsAny<string>()), Times.Exactly(3));
            mockClients.Verify(mc => mc.Group("Admins"), Times.Exactly(2));
            Assert.IsTrue(updateAdminTable && updateUserTable && updateUnreadTicketCounterAdmin &&
                updateUnreadTicketCounterUser && notifyAboutChangeTicketState);
        }

        [Test]
        public void NotifyAdminsAboutNewTicketTest()
        {
            //Arrange
            bool updateUserTable = false;
            bool updateAdminTable = false;
            bool updateUnreadTicketCounterAdmin = false;
            bool notifyAboutNewTicket = false;
            someUser.updateTicketTable = new Action(() =>
            {
                updateUserTable = true;
            });
            someGroup.updateTicketTable = new Action(() =>
            {
                updateAdminTable = true;
            });
            someGroup.updateUnreadTicketCounterForAdmin = new Action(() =>
            {
                updateUnreadTicketCounterAdmin = true;
            });
            someGroup.notifyAboutNewTicket = new Action<string>((subject) =>
            {
                notifyAboutNewTicket = true;
            });

            //Act
            hub.NotifyAdminsAboutNewTicket(It.IsAny<string>(), It.IsAny<string>());

            //Assert
            mockClients.Verify(mc => mc.User(It.IsAny<string>()), Times.Once);
            mockClients.Verify(mc => mc.Group("Admins"), Times.Exactly(3));
            Assert.IsTrue(updateAdminTable && updateUserTable && updateUnreadTicketCounterAdmin && notifyAboutNewTicket);
        }

        [Test]
        public void NotifyAboutSharedWordSuitesTest()
        {
            //Arrange
            string[] data = new string[]{"5", "2", "3"};
            bool notifyAboutSharedWordSuites = false;
            someUser.notifyAboutSharedWordSuites = new Action(() =>
            {
                notifyAboutSharedWordSuites = true;
            });

            //Act
            hub.NotifyAboutSharedWordSuites(data);

            //Assert
            mockClients.Verify(mc => mc.User(It.IsAny<string>()), Times.AtLeastOnce);
            Assert.IsTrue(notifyAboutSharedWordSuites);
        }

        [Test]
        public void OnConnectedTest_ReturnsTask_IfAdmin()
        {
            //Arrange
            bool updateUnreadTicketCounterForAdmin = false;
            Mock<HubCallerContext> mockContext = new Mock<HubCallerContext>();
            mockContext.Setup(mc => mc.ConnectionId).Returns(It.IsAny<string>());
            hub.Context = mockContext.Object;
            Mock<INameValueCollection> qs = new Mock<INameValueCollection>();
            qs.Setup(q => q.Get("role")).Returns("Admin");
            mockContext.Setup(mc => mc.QueryString).Returns(qs.Object);
            mockGroups.Setup(mg => mg.Add(It.IsAny<string>(), "Admins")).Verifiable();
            mockClients.Setup(mc => mc.Caller).Returns((ExpandoObject)someUser);
            someUser.updateUnreadTicketCounterForAdmin = new Func<Task>(() =>
            {
                return Task.Factory.StartNew(() =>
                {
                    updateUnreadTicketCounterForAdmin = true;
                });
            });

            //Act
            hub.OnConnected();

            //Assert
            mockContext.Verify(mc => mc.QueryString, Times.Exactly(2));
            mockContext.Verify(mc => mc.ConnectionId, Times.Once);
            qs.Verify(q => q.Get("role"), Times.Once);
            mockGroups.VerifyAll();
            mockClients.Verify(mc => mc.Caller, Times.Once);
            Assert.IsTrue(updateUnreadTicketCounterForAdmin);
        }

        [Test]
        public void OnConnectedTest_ReturnsTask_IfDoentAdmin()
        {
            //Arrange
            bool updateUnreadTicketCounterForUser = false;
            Mock<HubCallerContext> mockContext = new Mock<HubCallerContext>();
            hub.Context = mockContext.Object;
            Mock<INameValueCollection> qs = new Mock<INameValueCollection>();
            qs.Setup(q => q.Get("role")).Returns("Student");
            mockContext.Setup(mc => mc.QueryString).Returns(qs.Object);
            mockClients.Setup(mc => mc.Caller).Returns((ExpandoObject)someUser);
            someUser.updateUnreadTicketCounterForUser = new Func<Task>(() =>
            {
                return Task.Factory.StartNew(() =>
                {
                    updateUnreadTicketCounterForUser = true;
                });
            });

            //Act
            hub.OnConnected();

            //Assert
            mockContext.Verify(mc => mc.QueryString, Times.Exactly(2));
            qs.Verify(q => q.Get("role"), Times.Once);
            mockClients.Verify(mc => mc.Caller, Times.Once());
            Assert.IsTrue(updateUnreadTicketCounterForUser);
        }

        [Test]
        public void OnConnectedTest_ReturnsTask_IfAdminAndStudent()
        {
            //Arrange
            bool updateUnreadTicketCounterForAdmin = false;
            bool updateUnreadTicketCounterForUser = false;
            Mock<HubCallerContext> mockContext = new Mock<HubCallerContext>();
            mockContext.Setup(mc => mc.ConnectionId).Returns(It.IsAny<string>());
            hub.Context = mockContext.Object;
            Mock<INameValueCollection> qs = new Mock<INameValueCollection>();
            qs.Setup(q => q.Get("role")).Returns("Admin | Student");
            mockContext.Setup(mc => mc.QueryString).Returns(qs.Object);
            mockGroups.Setup(mg => mg.Add(It.IsAny<string>(), "Admins")).Verifiable();
            mockClients.Setup(mc => mc.Caller).Returns((ExpandoObject)someUser);
            someUser.updateUnreadTicketCounterForAdmin = new Func<Task>(() =>
            {
                return Task.Factory.StartNew(() =>
                {
                    updateUnreadTicketCounterForAdmin = true;
                });
            });
            someUser.updateUnreadTicketCounterForUser = new Func<Task>(() =>
            {
                return Task.Factory.StartNew(() =>
                {
                    updateUnreadTicketCounterForUser = true;
                });
            });

            //Act
            hub.OnConnected();

            //Assert
            mockContext.Verify(mc => mc.QueryString, Times.Exactly(2));
            mockContext.Verify(mc => mc.ConnectionId, Times.Once);
            qs.Verify(q => q.Get("role"), Times.Once);
            mockGroups.VerifyAll();
            mockClients.Verify(mc => mc.Caller, Times.Exactly(2));
            Assert.IsTrue(updateUnreadTicketCounterForAdmin && updateUnreadTicketCounterForUser);
        }

        [Test]
        public void OnConnectedTest_ReturnsTask_IfAdminAndTeacher()
        {
            //Arrange
            bool updateUnreadTicketCounterForAdmin = false;
            bool updateUnreadTicketCounterForUser = false;
            Mock<HubCallerContext> mockContext = new Mock<HubCallerContext>();
            mockContext.Setup(mc => mc.ConnectionId).Returns(It.IsAny<string>());
            hub.Context = mockContext.Object;
            Mock<INameValueCollection> qs = new Mock<INameValueCollection>();
            qs.Setup(q => q.Get("role")).Returns("Admin | Teacher");
            mockContext.Setup(mc => mc.QueryString).Returns(qs.Object);
            mockGroups.Setup(mg => mg.Add(It.IsAny<string>(), "Admins")).Verifiable();
            mockClients.Setup(mc => mc.Caller).Returns((ExpandoObject)someUser);
            someUser.updateUnreadTicketCounterForAdmin = new Func<Task>(() =>
            {
                return Task.Factory.StartNew(() =>
                {
                    updateUnreadTicketCounterForAdmin = true;
                });
            });
            someUser.updateUnreadTicketCounterForUser = new Func<Task>(() =>
            {
                return Task.Factory.StartNew(() =>
                {
                    updateUnreadTicketCounterForUser = true;
                });
            });

            //Act
            hub.OnConnected();

            //Assert
            mockContext.Verify(mc => mc.QueryString, Times.Exactly(2));
            mockContext.Verify(mc => mc.ConnectionId, Times.Once);
            qs.Verify(q => q.Get("role"), Times.Once);
            mockGroups.VerifyAll();
            mockClients.Verify(mc => mc.Caller, Times.Exactly(2));
            Assert.IsTrue(updateUnreadTicketCounterForAdmin && updateUnreadTicketCounterForUser);
        }
                
    }
}
