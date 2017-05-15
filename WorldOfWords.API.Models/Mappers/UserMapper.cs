using System;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class UserMapper : IUserMapper
    {
        public User Map(RegisterUserModel source)
        {
            return new User
            {
                Name = source.Login,
                Password = source.Password,
                Email = source.Email,
                Id = source.Id
            };
        }
    }
}
