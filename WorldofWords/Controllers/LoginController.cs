using System.Web.Http;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Services.IServices;

namespace WorldofWords.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IUserService _service;
        private readonly IUserToTokenMapper _mapUserToToken;

        public LoginController(IUserService service, IUserToTokenMapper mapUserToToken)
        {
            _service = service;
            _mapUserToToken = mapUserToToken;
        }

        [AllowAnonymous]
        public IHttpActionResult Post(UserWithPasswordModel userData)
        {
            if (_service.CheckUserAuthorization(userData))
            {
                return Ok(_mapUserToToken.MapToTokenModel(userData));
            }
            return Unauthorized();
        }
    }
}