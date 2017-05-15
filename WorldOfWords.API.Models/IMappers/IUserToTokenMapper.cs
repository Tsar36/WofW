using WorldOfWords.API.Models.Models;

namespace WorldOfWords.API.Models.IMappers
{
    public interface IUserToTokenMapper
    {
        TokenModel MapToTokenModel(UserWithPasswordModel userToMap);
    }
}
