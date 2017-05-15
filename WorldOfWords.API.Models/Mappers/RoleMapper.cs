using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfWords.API.Models
{
    public class RoleMapper: IRoleMapper
    {
        public RoleModel Map(Domain.Models.Role role)
        {
            return new RoleModel()
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public Domain.Models.Role Map(RoleModel role)
        {
            return new Domain.Models.Role()
            {
                Id = role.Id,
                Name = role.Name
            };
        }
    }
}
