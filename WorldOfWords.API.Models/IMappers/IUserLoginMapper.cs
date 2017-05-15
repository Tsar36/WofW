using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IUserLoginMapper
    {
        LoggedUserModel FromUserToUserLoginApi(User userToMapp);
    }
}