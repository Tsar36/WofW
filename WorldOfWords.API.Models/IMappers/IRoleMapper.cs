using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IRoleMapper
    {
        RoleModel Map(Role role);
        Role Map(RoleModel role);
    }
}
