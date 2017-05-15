using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfWords.API.Models
{
    public class UserForListOfUsersMapper : IUserForListOfUsersMapper
    {
        IRoleMapper _roleMapper;

        public UserForListOfUsersMapper(IRoleMapper roleMapper)
        {
            _roleMapper = roleMapper;
        }

        public UserForListOfUsersModel Map(Domain.Models.User user)
        {
            return new UserForListOfUsersModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Roles = user.Roles.Select(item => _roleMapper.Map(item)).ToList()
            };

        }

        public Domain.Models.User Map(UserForListOfUsersModel user)
        {
            return new Domain.Models.User
            {
                Roles = user.Roles.Select(item => _roleMapper.Map(item)).ToList(),
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            };
        }
    }
}
