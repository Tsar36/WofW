using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Validation;

namespace WorldOfWords.API.Models.Mappers
{
    public class UserToTokenMapper: IUserToTokenMapper
    {
        private readonly ITokenValidation _token;

        public UserToTokenMapper(ITokenValidation token)
        {
            _token = token;
        }

        public TokenModel MapToTokenModel(UserWithPasswordModel userToMap)
        {
            return new TokenModel()
            {
                EmailAndIdToken = _token.EncodeEmailAndIdToken(userToMap.Id.ToString() + ' ' + userToMap.Email.ToLower()),
                RolesToken = _token.EncodeRoleToken(userToMap.Roles),
                HashToken = userToMap.HashToken
            };
        }
    }
}
