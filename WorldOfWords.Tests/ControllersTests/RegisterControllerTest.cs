using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using System.Web.Http.Results;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using WorldofWords.Controllers;
using WorldOfWords.API.Models;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services;
using WorldOfWords.Validation;
using Moq;
using WorldOfWords.Domain.Services.IServices;
using System.Security.Principal;
using System.Threading;
using System.Web.Http;

namespace WorldOfWords.Tests.ControllersTests
{
    [TestFixture]
    public class RegisterControllerTest
    {
        private void GenerateData(string name, string[] roles)
        {
            GenericIdentity identity = new GenericIdentity(name);
            Thread.CurrentPrincipal =
                new GenericPrincipal(
                    identity,
                    roles
                    );
        }

        Mock<IUserService> userService = new Mock<IUserService>();
        Mock<IUserMapper> userMapper = new Mock<IUserMapper>();
        Mock<ITokenValidation> tokenValidation = new Mock<ITokenValidation>();
        Mock<IIncomingUserMapper> incUserMapper = new Mock<IIncomingUserMapper>();
        Mock<IIdentityMessageService> mesService = new Mock<IIdentityMessageService>();
        Mock<UrlHelper> urlHelper = new Mock<UrlHelper>();

        [Test]
        public async Task Post_Register_Should_Return_Ok_TokenModel()
        {
            //Arrange
            var registerUser = new RegisterUserModel
            {
                Login = "Cat-Ok",
                Email = "roman@example.com",
                Password = "3452",
                Id = 0
            };

            var incomingUser = new IncomingUser
            {
                Name = "Cat-Ok",
                Email = "roman@example.com",
                Password = "3452",
                Token = "asdf",
                Id = 1
            };

            var user = new User
            {
                Name = "Roman Hapatyn",
                Email = "roman@example.com",
                Password = "3452",
                HashedToken = "asdf",
                Id = 1
            };

            TokenModel token = new TokenModel();
            token.RolesToken = "Student";

            //Action
            
            GenerateData("1", new[] { "NoRoles" });
            RegisterController registerController = new RegisterController(userService.Object,
                tokenValidation.Object,
                incUserMapper.Object,
                mesService.Object);
            var tokenModel = new TokenModel();
            string locationUrl = "http://location/";

            var message = new IdentityMessage
                {
                    Body = "Please confirm your account at: www.xxx.xxx ",
                    Destination = "georgeproniuk@gmail.com",
                    Subject = "Confirm your registration"
                };

            urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(locationUrl);
            registerController.Url = urlHelper.Object;

            mesService
                .Setup(x => x.SendAsync(message))
                .Returns(Task.FromResult(0));

            incUserMapper
                .Setup(x => x.ToIncomingUser(registerUser))
                .Returns(incomingUser);

            userMapper
                .Setup(x => x.Map(registerUser))
                .Returns(user);

            userService
                .Setup(x => x.Exists(incomingUser))
                .Returns(false);

            tokenValidation
                .Setup(x => x.GetHashSha256("asdf"))
                .Returns("fdsa");

            var actual = await registerController.Post(registerUser) as OkNegotiatedContentResult<TokenModel>;
            var contentResult = actual as OkNegotiatedContentResult<TokenModel>;
            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Content.GetType(), tokenModel.GetType());
        }

        [Test]
        public async Task Post_Register_Should_Return_BadRequest()
        {
            //Arrange
            var registerUser = new RegisterUserModel
            {
                Login = "Cat-Ok",
                Email = "roman@example.com",
                Password = "3452",
                Id = 0
            };

            var incomingUser = new IncomingUser
            {
                Name = "Cat-Ok",
                Email = "roman@example.com",
                Password = "3452",
                Token = "asdf",
                Id = 1
            };

            var user = new User
            {
                Name = "Roman Hapatyn",
                Email = "roman@example.com",
                Password = "3452",
                HashedToken = "asdf",
                Id = 1
            };

            TokenModel token = new TokenModel();
            token.RolesToken = "Student";

            //Action
            GenerateData("1", new[] { "NoRoles" });
            RegisterController registerController = new RegisterController(userService.Object,
                tokenValidation.Object,
                incUserMapper.Object,
                mesService.Object);
            var tokenModel = new TokenModel();
            string locationUrl = "http://location/";

            var message = new IdentityMessage
            {
                Body = "Please confirm your account at: www.xxx.xxx ",
                Destination = "georgeproniuk@gmail.com",
                Subject = "Confirm your registration"
            };

            urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(locationUrl);
            registerController.Url = urlHelper.Object;

            mesService
                .Setup(x => x.SendAsync(message))
                .Returns(Task.FromResult(0));
           
            incUserMapper
                .Setup(x => x.ToIncomingUser(registerUser))
                .Returns(incomingUser);

            userMapper
                .Setup(x => x.Map(registerUser))
                .Returns(user);

            userService
                .Setup(x => x.Exists(incomingUser))
                .Returns(true);

            tokenValidation
                .Setup(x => x.GetHashSha256("asdf"))
                .Returns("fdsa");

            var actual = await registerController.Post(registerUser);
            var badRequestResult = new BadRequestResult(registerController);
            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.GetType(), badRequestResult.GetType());
        }

        [Test]
        public void Get_ConfirmEmail_Should_Return_Ok()
        {
            //Action
            GenerateData("1", new[] { "NoRoles" });
            RegisterController registerController = new RegisterController(userService.Object,
                tokenValidation.Object,
                incUserMapper.Object,
                mesService.Object);

            int userId = 1;
            string code = "test";

            userService.Setup(x => x.ConfirmUserRegistration(userId, code))
                .Returns(true);

            var actual = registerController.ConfirmEmail(userId, code);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf(typeof(OkResult), actual);
        }

        [Test]
        public void Get_ConfirmEmail_Should_Return_BadRequest_ModelState()
        {
            //Action
            GenerateData("1", new[] { "NoRoles" });
            RegisterController registerController = new RegisterController(userService.Object,
                tokenValidation.Object,
                incUserMapper.Object,
                mesService.Object);

            //Arrange
            int userId = 0;
            string code = "";

            Assert.AreEqual(userId, 0);
            Assert.AreEqual(code, "");

            var actual = registerController.ConfirmEmail(userId, code);
            var badRequestResult = new BadRequestResult(registerController);
            InvalidModelStateResult invalidModel = new InvalidModelStateResult(registerController.ModelState, registerController);
            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.GetType(), invalidModel.GetType());
        }

        [Test]
        public void Get_ConfirmEmail_Should_Return_BadRequest()
        {
            //Action
            GenerateData("1", new[] { "NoRoles" });
            RegisterController registerController = new RegisterController(userService.Object,
                tokenValidation.Object,
                incUserMapper.Object,
                mesService.Object);

            //Arrange
            int userId = 1;
            string code = "test";

            var actual = registerController.ConfirmEmail(userId, code);
            var badRequestResult = new BadRequestErrorMessageResult("Invalid data", registerController);
            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.GetType(), badRequestResult.GetType());
        }

        [Test]
        public async Task Post_ForgotPassword_Should_Return_Ok_TokenModel()                                               
        {
            //Arrange
            var forgotPasswordUser = new ForgotPasswordUserModel
            {
                Email = "georgeproniuk@gmail.com",
                Id = "1",
                Password = "test"
            };

            var message = new IdentityMessage
            {
                Body = "Reset pasword at test.com",
                Destination = "georgeproniuk@gmail.com",
                Subject = "Password reset"
            };

            //Action
            GenerateData("1", new[] { "NoRoles" });
            RegisterController registerController = new RegisterController(userService.Object,
                tokenValidation.Object,
                incUserMapper.Object,
                mesService.Object);
            var tokenModel = new TokenModel();
            string locationUrl = "http://location/";

            urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(locationUrl);
            registerController.Url = urlHelper.Object;

            mesService
                .Setup(x => x.SendAsync(message))
                .Returns(Task.FromResult(0));

            userService.Setup(x => x.CheckUserEmail(forgotPasswordUser))
                .Returns(true);
           
            tokenValidation.Setup(x => x.GetHashSha256("asdf"))
                .Returns("fdsa");
                
            var actual = await registerController.ForgotPassword(forgotPasswordUser) as OkNegotiatedContentResult<TokenModel>;
            var contentResult = actual as OkNegotiatedContentResult<TokenModel>;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Content.GetType(), tokenModel.GetType());
        }
    }
}
