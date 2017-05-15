using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models.Mappers
{
    public class IncomingUserMapper : IIncomingUserMapper
    {
        public IncomingUser ToIncomingUser(RegisterUserModel apiModel)
        {
            return new IncomingUser
            {
                Email = apiModel.Email,
                Name = apiModel.Login,
                LanguageId = apiModel.LanguageId,
                Password = apiModel.Password,
                PagesUrl = apiModel.PagesUrl
            };
        }


        public User ToDomainModel(IncomingUser incomingUser)
        {
            return new User
            {
                Id = incomingUser.Id,
                Name = incomingUser.Name,
                Email = incomingUser.Email,
                Password = incomingUser.Password,
                LanguageId = incomingUser.LanguageId
            };
        }
    }
}
