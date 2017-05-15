using System;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using WorldOfWords.API.Models;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Validation;
using WorldOfWords.Domain.Services.MessagesAndConsts;

namespace WorldofWords.Controllers
{
    [RoutePrefix("api/register")]
    public class RegisterController : BaseController
    {
        private readonly IIncomingUserMapper _incomingUserMapper;
        private readonly IUserService _service;
        private readonly ITokenValidation _token;
        private readonly TokenModel _tokenModel;
        private readonly IIdentityMessageService _emailService;

        public RegisterController(IUserService userService,
            ITokenValidation token, IIncomingUserMapper incomingUserMapper,
            IIdentityMessageService emailService)
        {
            _service = userService;
            _token = token;
            _incomingUserMapper = incomingUserMapper;
            _emailService = emailService;
            _tokenModel = new TokenModel();
        }

        [AllowAnonymous]
        public async Task<IHttpActionResult> Post(RegisterUserModel args)
        {
            var newUser = _incomingUserMapper.ToIncomingUser(args);
            if (!_service.Exists(newUser))
            {
                _service.Add(newUser);

                var code = GenerateEmailConfirmationToken(newUser.Id.ToString(), true);
                newUser.Token = code;
                _service.AddToken(newUser);

                await SendEmailConfirmationAsync(code, newUser.Email, "Registration confirmation at WoW", newUser.PagesUrl, newUser.Id);

                return Ok(_tokenModel);
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public IHttpActionResult ConfirmEmail(int userId = 0, string code = "")
        {
            bool isEmptyUserId = (userId == 0);
            if (isEmptyUserId || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }
            if (_service.ConfirmUserRegistration(userId, code))
            {
                return Ok();
            }
            return BadRequest("Invalid data");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordUserModel model)
        {
            if (_service.CheckUserEmail(model))
            {
                var code = GenerateEmailConfirmationToken(model.Id.ToString(), false);

                await SendEmailConfirmationAsync(code, model.Email, "Password reset at WoW", model.PagesUrl);

                return Ok(_tokenModel);
            }

            return BadRequest();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ChangePassword")]
        public IHttpActionResult ChangePassword(ForgotPasswordUserModel model)
        {
            if (_service.CheckUserId(model))
            {
                _service.ChangePassword(model);
                return Ok(_tokenModel);
            }
            return BadRequest();
        }

        private string GenerateEmailConfirmationToken(string id, bool emailConfirm)
        {
            string result = "";
            if (emailConfirm)
            {
                Guid randomPart = Guid.NewGuid();
                var tokenToHash = randomPart.ToString() + id;
                _tokenModel.EmailConfirmationToken = _token.GetHashSha256(tokenToHash);
                result = _tokenModel.EmailConfirmationToken;
            }
            else
            {
                _tokenModel.ForgotPasswordToken = _token.GetHashSha256(id);
                result = _tokenModel.ForgotPasswordToken;
            }

            return result;
        }

        private async Task SendEmailConfirmationAsync(string code, string email, string subject, string pagesUrl, int id = 0)
        {
            var  url = pagesUrl + "/Index";
            string mainPart = id != 0 ? MessagesContainer.ConfiramtionMessage : MessagesContainer.ForgotPasswordMessage;
            string redirectPart = id != 0 ? String.Format("#/EmailConfirmed?id={0}&code={1}", id, code) : String.Format("#/ChangePassword?id={0}", code);

            await (_emailService).SendAsync(new IdentityMessage
            {
                Body = String.Format("{0}\n{1}{2}", mainPart, url, redirectPart),
                Destination = email,
                Subject = subject
            });
        }
    }
}
