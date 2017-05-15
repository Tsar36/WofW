using System.Security.Principal;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using WorldofWords.Controllers;
using WorldOfWords.Domain.Services.IServices;

namespace WorldOfWords.Tests.ControllersTests
{
    [TestFixture]
    public class EditUserControllerTest
    {
        readonly Mock<IUserService> _userService = new Mock<IUserService>();
        private EditUserProfileController _editUser;
        
        private void GenerateData(string name, string[] roles)
        {
            GenericIdentity identity = new GenericIdentity(name);
            Thread.CurrentPrincipal =
                new GenericPrincipal(
                    identity,
                    roles
                    );
            _editUser = new EditUserProfileController(_userService.Object);
        }

        [Test]
        public void GetNameEditCtrlTest()
        {
            GenerateData("1", new[] {"NoRoles"});
            _userService.Setup(u => u.GetUserName(1)).Returns("Roman");

            IHttpActionResult okResult = _editUser.Get();
            var actionOkResult = okResult as OkNegotiatedContentResult<string>;

            GenerateData("", new[] {"NoRoles"});
            _userService.Setup(u => u.GetUserName(0)).Returns("");
            IHttpActionResult badResult = _editUser.Get();
            var actionBadResult = badResult as BadRequestResult;

            Assert.IsNotNull(actionBadResult);
            Assert.IsNotNull(actionOkResult);
            var okContent = actionOkResult.Content;
            Assert.AreEqual(okContent,"Roman");
        }

        [Test]
        public void EditNameEditCtrlTest()
        {
            GenerateData("1", new[] { "NoRoles" });
            const string newUserName = "NewName";
            _userService.Setup(u => u.EditUserName(newUserName, 1)).Returns(true);

            IHttpActionResult okResult = _editUser.Post(newUserName);
            var actionOkResult = okResult as OkResult;

            _userService.Setup(u => u.EditUserName(newUserName, 1)).Returns(false);

            IHttpActionResult unauthorizeResult = _editUser.Post(newUserName);
            var actionUnauthorizeResult = unauthorizeResult as BadRequestResult;

            Assert.IsNotNull(actionOkResult);
            Assert.IsNotNull(actionUnauthorizeResult);
        }

        [Test]
        public void EditPasswordEditCtrlTest()
        {
            GenerateData("1", new[] {"NoRoles"});
            const string newPassword = "1111";
            _userService.Setup(u => u.EditUserPassword(newPassword, 1)).Returns(true);

            IHttpActionResult okResult = _editUser.PostPassword(newPassword);
            var actionOkResult = okResult as OkResult;

            _userService.Setup(u => u.EditUserPassword(newPassword, 1)).Returns(false);

            IHttpActionResult unauthorizeResult = _editUser.PostPassword(newPassword);
            var actionUnauthorizeResult = unauthorizeResult as BadRequestResult;

            Assert.IsNotNull(actionOkResult);
            Assert.IsNotNull(actionUnauthorizeResult);
        }

        [Test]
        public void CheckNameEditCtrlTest()
        {
            GenerateData("1", new[] { "NoRoles" });
            const string name = "Roman";
            _userService.Setup(u => u.CheckUserName(name, 1)).Returns(true);

            IHttpActionResult okResult = _editUser.PostCheckName(name);
            var actionOkResult = okResult as OkResult;

            const string badName = "NoName";
            _userService.Setup(u => u.CheckUserPassword(badName, 1)).Returns(false);

            IHttpActionResult badResult = _editUser.PostCheckPassword(badName);
            var actionBadResult = badResult as BadRequestResult;

            _userService.Setup(u => u.CheckUserName(name, 1)).Returns(false);

            IHttpActionResult unauthorizeResult = _editUser.PostCheckName(name);
            var actionUnauthorizeResult = unauthorizeResult as BadRequestResult;

            Assert.IsNotNull(actionBadResult);
            Assert.IsNotNull(actionOkResult);
            Assert.IsNotNull(actionUnauthorizeResult);
        }

        [Test]
        public void CheckPasswordEditCtrlTest()
        {
            GenerateData("1", new[] { "NoRoles" });
            const string password = "5422";
            _userService.Setup(u => u.CheckUserPassword(password, 1)).Returns(true);

            IHttpActionResult okResult = _editUser.PostCheckPassword(password);
            var actionOkResult = okResult as OkResult;

            const string badPasword = "1111";
            _userService.Setup(u => u.CheckUserPassword(badPasword, 1)).Returns(false);

            IHttpActionResult badResult = _editUser.PostCheckPassword(badPasword);
            var actionBadResult = badResult as BadRequestResult;

            _userService.Setup(u => u.CheckUserPassword(password, 1)).Returns(false);

            IHttpActionResult unauthorizeResult = _editUser.PostCheckPassword(password);
            var actionUnauthorizeResult = unauthorizeResult as BadRequestResult;

            Assert.IsNotNull(actionBadResult);
            Assert.IsNotNull(actionOkResult);
            Assert.IsNotNull(actionUnauthorizeResult);
        }
    }
}
