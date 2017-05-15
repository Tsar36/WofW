using System.Collections.Generic;
using System.Web.Http;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Services;
using System.Linq;
using System.Web.Http.Results;
using WorldOfWords.Domain.Services.IServices;


namespace WorldofWords.Controllers
{
    [WowAuthorization(Roles = "Admin")]
    public class UsersListController : ApiController
    {
        private readonly IUserForListOfUsersMapper _usermapper;
        private readonly IUserService _userservice;

        public UsersListController(IUserForListOfUsersMapper usermapper, IUserService userservice)
        {
            _usermapper = usermapper;
            _userservice = userservice;
        }

        public List<UserForListOfUsersModel> Get()
        {
            return _userservice.GetAllUsers().Select(item => _usermapper.Map(item)).ToList();
        }

        public List<UserForListOfUsersModel> Get(int start, int end, int roleid)
        {
            var users = _userservice.GetUsersFromIntervalByRoleId(start, end, roleid);
            return _userservice.GetUsersFromIntervalByRoleId(start, end, roleid).Select(item => _usermapper.Map(item)).ToList();
        }

        public int Get(int roleId)
        {
            return _userservice.GetAmountOfUsersByRoleId(roleId);
        }

        public List<UserForListOfUsersModel> Get(string namevaluetosearch, int roleid)
        {
            return _userservice.SearchByNameAndRole(namevaluetosearch, roleid).Select(item => _usermapper.Map(item)).ToList();
        }

        public IHttpActionResult Put(UserForListOfUsersModel user)
        {
            if (_userservice.ChangeRolesOfUser(_usermapper.Map(user)))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            };
        }
    }
}
