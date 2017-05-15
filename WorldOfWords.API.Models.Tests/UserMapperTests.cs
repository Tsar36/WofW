using NUnit.Framework;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models.Tests
{
    [TestFixture]
    public class UserMapperTests
    {
        [Test]
        public void Map_FieldsAreEqual()
        {
            //Arrange
            var initial = new RegisterUserModel
            {
                Email = "roman@example.com",
                Login = "Roman",
                Password = "5422"
            };
            var expected = new User
            {
                Email = "roman@example.com",
                Name = "Roman",
                Password = "5422"
            };
            //Act
            var actual = (new UserMapper()).Map(initial);
            //Assert
            Assert.AreEqual(expected.Email, actual.Email);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Password, actual.Password);
        }
    }
}
