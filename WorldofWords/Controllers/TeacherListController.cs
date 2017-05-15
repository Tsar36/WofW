using System.Collections.Generic;
using System.Web.Http;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Services;
using System.Linq;
using System.Web.Http.Results;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Domain.Models;
using System.Threading.Tasks;

namespace WorldofWords.Controllers
{
    [WowAuthorization(Roles = "Teacher")]
    [RoutePrefix("api/TeacherList")]
    public class TeacherListController : BaseController
    {
        private readonly IUserForListingMapper _userListMapper;
        private readonly IUserService _userservice;

        public TeacherListController(IUserService userservice, IUserForListingMapper userListMapper)
        {
            _userservice = userservice;
            _userListMapper = userListMapper; 
        }

        [Route("GetByRoleId")]
        //mfomitc
        public async Task<IEnumerable<UserForListingModel>> GetByRoleIdAsync(int roleId)
        {

            return await _userservice.GetUsersByRoleIdAsync(roleId);

            //return _userservice.GetUsersByRoleIdAsync(roleId).Select(item => _userListMapper.Map(item)).ToList();
        }
    }
}
