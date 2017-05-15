using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IIncomingUserMapper
    {
        IncomingUser ToIncomingUser(RegisterUserModel apiModel);
        User ToDomainModel(IncomingUser incomingUser);
    }
}
