using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IUserMapper
    {
        User Map(RegisterUserModel source);
    }
}
