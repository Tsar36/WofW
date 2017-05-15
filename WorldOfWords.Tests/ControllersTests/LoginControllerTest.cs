using System.Security.Principal;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using WorldofWords.Controllers;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Services.IServices;

namespace WorldOfWords.Tests.ControllersTests
{
    [TestFixture]
    public class LoginControllerTest
    {
        [Test]
        public void LoginControllerPostMethodTest()
        {
            //Arrange
            var userNotInDb = new UserWithPasswordModel
            {
                Email = "roman@example.com",
                Password = "3452"
            };
            var userInDb = new UserWithPasswordModel
            {
                Email = "roman@example.com",
                Password = "5422",
                Roles = new[] { "Student", "Teacher" }
            };
            var userWithoutEmail = new UserWithPasswordModel
            {
                Email = null,
                Password = "NotNull"
            };

            //Action            
            Mock<IUserService> userService = new Mock<IUserService>();
            Mock<IUserToTokenMapper> tokenMaper = new Mock<IUserToTokenMapper>();
            GenericIdentity identity = new GenericIdentity("1");
            Thread.CurrentPrincipal =
                new GenericPrincipal(
                    identity,
                    new[] { "NoRoles" }
                    );
            LoginController loginController = new LoginController(userService.Object, tokenMaper.Object);

            userService
                .Setup(x => x.CheckUserAuthorization(userWithoutEmail))
                .Returns(false);

            IHttpActionResult unauthorizedResult = loginController.Post(userNotInDb);
            var actionUnauthorizedResult = unauthorizedResult as UnauthorizedResult;
            unauthorizedResult = loginController.Post(userWithoutEmail);
            var actionUnauthorizedRequestEmail = unauthorizedResult as UnauthorizedResult;

            userService
                .Setup(x => x.CheckUserAuthorization(userInDb))
                .Returns(true);

            tokenMaper
                .Setup(x => x.MapToTokenModel(userInDb))
                .Returns(new TokenModel()
                {
                    RolesToken = "Claim token",
                    EmailAndIdToken = "Email and password token"
                });

            IHttpActionResult authorizedResult = loginController.Post(userInDb);
            var actionAuthorizedResult = authorizedResult as OkNegotiatedContentResult<TokenModel>;
            
            IHttpActionResult badRequestResult = loginController.Post(null);
            var actionBadRequest = badRequestResult as UnauthorizedResult;

            //Assert
            Assert.IsNotNull(actionUnauthorizedRequestEmail);
            Assert.IsNotNull(actionBadRequest);
            Assert.IsNotNull(actionUnauthorizedResult);
            Assert.IsNotNull(actionAuthorizedResult);
            var authorizedContent = actionAuthorizedResult.Content;
            Assert.IsNotNull(actionAuthorizedResult.Content);
            Assert.AreEqual(authorizedContent.RolesToken, "Claim token");
        }
    }
}
