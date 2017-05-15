using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using WorldOfWords.API.Models.Mappers;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Validation;

namespace WorldOfWords.API.Models.Tests
{
    [TestFixture]
    public class UserToTokenMapperTest
    {
        [Test]
        public void MapToTokenModelTest()
        {
            const string email = "roman@example.com";
            const int id = 1;
            const string hash = "HashToken";
            IEnumerable<string> roles = new[] {"Admin", "Student", "Teacher"};
            Mock<ITokenValidation> token = new Mock<ITokenValidation>();
            UserToTokenMapper mapper = new UserToTokenMapper(token.Object);

            var initial = new UserWithPasswordModel()
            {
                Email = email,
                Id = id,
                HashToken = hash,
                Roles = roles
            };
            var expected = new TokenModel()
            {
                EmailAndIdToken = email,
                RolesToken = roles.ToString(),
                HashToken = hash
            };

            token.Setup(x => x.EncodeEmailAndIdToken(id.ToString() + ' ' + email.ToLower()))
                .Returns(email);

            token.Setup(x => x.EncodeRoleToken(roles))
                .Returns(roles.ToString);

            var action = mapper.MapToTokenModel(initial);
            Assert.IsNotNull(action);
            Assert.AreEqual(action.EmailAndIdToken, expected.EmailAndIdToken);
            Assert.AreEqual(action.HashToken, expected.HashToken);
            Assert.AreEqual(action.RolesToken, expected.RolesToken);
        }
    }
}
