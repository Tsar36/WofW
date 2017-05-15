using System;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class UserLoginMapper : IUserLoginMapper
    {
        public LoggedUserModel FromUserToUserLoginApi(User userToMapp)
        {
            LoggedUserModel userLoginApi = new LoggedUserModel();
            userLoginApi.Name = userToMapp.Name;
            userLoginApi.Id = userToMapp.Id;
            userLoginApi.EMail = userToMapp.Email;
            return userLoginApi;
        }
    }
}
