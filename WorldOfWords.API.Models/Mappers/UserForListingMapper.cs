using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class UserForListinigMapper : IUserForListingMapper
    {
        public User Map(UserForListingModel userModel)
        {
            return new User
            {
                Id = userModel.Id,
                Name = userModel.Name
            };
        }

        public List<User> Map(List<UserForListingModel> userModels)
        {
            List<User> users = new List<User>();
            foreach (var userModel in userModels)
            {
                users.Add(Map(userModel));
            }
            return users;
        }

        public UserForListingModel Map(User user)
        {
            return new UserForListingModel()
            {
                Id = user.Id,
                Name = user.Name
            };
        }

        public List<UserForListingModel> Map(List<User> users)
        {
            List<UserForListingModel> userModels = new List<UserForListingModel>();
            foreach (var user in users)
            {
                userModels.Add(Map(user));
            }
            return userModels;
        }
    }
}
