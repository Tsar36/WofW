using NUnit.Framework;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models.Tests
{
    [TestFixture]
    public class UserLoginMapperTests
    {
        [Test]
        public void FromUserToUserLoginApi_FieldsAreEqual()
        {
            //Arrange
            var initial = new User
            {
                Id = 1,
                Email = "roman@example.com",
                Name = "Roman",
                Password = "5422"
            };
            var expected = new LoggedUserModel
            {
                EMail = "roman@example.com",
                Name = "Roman",
                Id = 1
            };
            //Act
            var actual = (new UserLoginMapper()).FromUserToUserLoginApi(initial);
            //Assert
            Assert.AreEqual(expected.EMail, actual.EMail);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Id, actual.Id);
        }
    }
}
