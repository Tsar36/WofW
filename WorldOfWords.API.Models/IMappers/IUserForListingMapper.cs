using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IUserForListingMapper
    {
        User Map(UserForListingModel userModel);
        List<User> Map(List<UserForListingModel> userModels);
        UserForListingModel Map(User user);
        List<UserForListingModel> Map(List<User> users);
    }
}
