using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;
using System.Collections;
using System.Collections.Generic;
using WorldOfWords.API.Models;
using System.Threading.Tasks;

namespace WorldOfWords.Domain.Services.IServices
{
    public interface IUserService
    {
        void Add(IncomingUser user);
        void AddToken(IncomingUser user);
        bool Exists(IncomingUser user);
        bool CheckUserAuthorization(UserWithPasswordModel userLoginApi);
        bool ConfirmUserRegistration(int userId, string token);
        bool CheckUserName(string checkName, int userId);
        bool CheckUserPassword(string checkPassword, int userId);
        bool CheckUserEmail(ForgotPasswordUserModel checkEmail);
        bool CheckUserId(ForgotPasswordUserModel checkEmail);
        bool EditUserName(string newName, int userId);
        bool EditUserPassword(string newPassword, int userId);
        void ChangePassword(ForgotPasswordUserModel model);
        string GetUserName(int userId);
        List<User> GetAllUsers();
        //mfomitc
        Task<IEnumerable<UserForListingModel>> GetUsersByRoleIdAsync(int roleId);
        int GetAmountOfUsersByRoleId(int roleId = 0);
        List<User> GetUsersFromIntervalByRoleId(int startOfInterval, int endOfInterval, int roleId = 0);
        bool ChangeRolesOfUser(User user);
        List<User> SearchByNameAndRole(string name, int roleid = 0);
        IEnumerable<string> GetCoursesNamesByUserId(int userId);
    }
}