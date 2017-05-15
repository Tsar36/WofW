using System.Web.Http;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Domain.Services.MessagesAndConsts;

namespace WorldofWords.Controllers
{
    [WowAuthorization(AllRoles = new[] { "Teacher", "Student", "Admin" })]
    [RoutePrefix("api/EditUserProfile")]
    public class EditUserProfileController : BaseController
    {
        private readonly IUserService _userService;

        public EditUserProfileController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("GetName")]
        public IHttpActionResult Get()
        {
            var userName = _userService.GetUserName(UserId);
            if (!string.IsNullOrEmpty(userName))
                return Ok(userName);
            return BadRequest();
        }

        [HttpGet]
        [Route("GetNameById")]
        public IHttpActionResult Get(int userId)
        {
            var userName = _userService.GetUserName(userId);
            if (!string.IsNullOrEmpty(userName))
                return Ok(userName);
            return BadRequest();
        }

       
        [Route("EditName")]
        public IHttpActionResult Post(string editName)
        {
            if (string.IsNullOrEmpty(editName) 
                && UserId != ConstContainer.BadId)
                return BadRequest();

            if (_userService.EditUserName(editName, UserId))
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("EditPassword")]
        public IHttpActionResult PostPassword(string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword)
                && UserId != ConstContainer.BadId)
                return BadRequest();

            if (_userService.EditUserPassword(newPassword, UserId))
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("CheckName")]
        public IHttpActionResult PostCheckName(string checkName)
        {
            if (string.IsNullOrEmpty(checkName)
                && UserId != ConstContainer.BadId)
                return BadRequest();

            if (_userService.CheckUserName(checkName, UserId))
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("CheckPassword")]
        public IHttpActionResult PostCheckPassword(string checkPassword)
        {
            if (string.IsNullOrEmpty(checkPassword)
                && UserId != ConstContainer.BadId)
                return BadRequest();

            if (_userService.CheckUserPassword(checkPassword, UserId))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}