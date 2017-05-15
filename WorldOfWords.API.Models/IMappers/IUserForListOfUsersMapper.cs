using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IUserForListOfUsersMapper
    {
        UserForListOfUsersModel Map(User user);
        Domain.Models.User Map(UserForListOfUsersModel user);
    }
}
