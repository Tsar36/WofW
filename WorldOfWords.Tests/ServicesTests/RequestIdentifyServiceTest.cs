using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using System.Text;
using System.Threading.Tasks;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.Domain.Services.Services;
using WorldOfWords.Infrastructure.Data.EF.UnitOfWork;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Tests.ServicesTests
{
    [TestFixture]
    class RequestIdentifyServiceTest
    {
        //UnitTest-Bohdan Ivanychko
        //Review-Yurii Harasymovych
        Mock<IUnitOfWorkFactory> _unitOfWork;
        Mock<IWorldOfWordsUow> _uow;
        Mock<IUserRepository> rep;
        RequestIdentityService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWorkFactory>();
            _uow = new Mock<IWorldOfWordsUow>();
            rep = new Mock<IUserRepository>();
            _service = new RequestIdentityService(_unitOfWork.Object);

            _unitOfWork.Setup(f => f.GetUnitOfWork()).Returns(_uow.Object);
            _uow.Setup(u => u.UserRepository).Returns(rep.Object);
        }

        [Test]
        public void CheckIdentity_ReturnsTrue_Positive()
        {
            //Arrange
            string hasFromRequest = "6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b";
            string hashedToken = "pesik";
            string id = "1";
            int idd;
            string userHashedToken = "pesik";
            bool expected = true;
            string[] roles = new string[2];
            IQueryable<User> users = new List<User>
                                {
                                    new User 
                                    {
                                        Id = int.Parse(id),
                                        HashedToken = userHashedToken
                                    }
                                }.AsQueryable<User>();
            rep.Setup(r => r.GetAll()).Returns(users);
            //Act
            var actual = _service.CheckIdentity(hasFromRequest, hashedToken, roles, out id);

            //Assert
            _unitOfWork.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.UserRepository, Times.Once);
            rep.Verify(r => r.GetAll(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckIdentity_ReturnsTrue_Null()
        {
            //Arrange
            string hasFromRequest = "6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b";
            string hashedToken = "pesik";
            string id = null;
            int idd;
            int.TryParse(id, out idd);
            string userHashedToken = "pesik";
            bool expected = false;
            string[] roles = new string[2];
            IQueryable<User> users = new List<User>
                                {
                                    new User 
                                    {
                                        Id=idd,
                                        HashedToken = userHashedToken
                                    }
                                }.AsQueryable<User>();
            rep.Setup(r => r.GetAll()).Returns(users);
            //Act
            var actual = _service.CheckIdentity(hasFromRequest, hashedToken, roles, out id);

            //Assert
            _unitOfWork.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.UserRepository, Times.Once);
            rep.Verify(r => r.GetAll(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckHash_ReturnsTrue_Positive()
        {
            //Arrange
            string hasFromRequest = "6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b";
            string hashedToken = "pesik";
            string id = "1";
            string userHashedToken = "pesik";
            bool expected = true;

            //Act
            var actual = _service.CheckHash(hasFromRequest, hashedToken, id, userHashedToken);

            //Assert
            Assert.AreEqual(expected, actual);
            
        }

        [Test]
        public void CheckHash_ReturnsTrue_Negative()
        {
            //Arrange
            string hasFromRequest = "6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b";
            string hashedToken = "pesik";
            string id = "1";
            string userHashedToken = "pesik";
            bool expected = false;

            //Act
            var actual = _service.CheckHash(hasFromRequest, hashedToken, id, userHashedToken);

            //Assert
            Assert.AreNotEqual(expected, actual);

        }

        [Test]
        public void ChechIdentityWithTwoParameters_Positive_UserExists()
        {
            //Arrange
            string hashedToken = "pes";
            string id = "8";
            rep.Setup(r => r.GetById(It.IsAny<int>())).Returns(new User
            {
                HashedToken = hashedToken
            });

            //Act
            bool actual = _service.CheckIdentity(hashedToken, id);

            //Assert
            _unitOfWork.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.UserRepository, Times.Once);
            rep.Verify(r => r.GetById(It.IsAny<int>()), Times.Once);
            Assert.IsTrue(actual);
        }

        [Test]
        public void ChechIdentityWithTwoParameters_TrowsException_UserDoesntExist()
        {
            //Arrange
            string hashedToken = "pes";
            string id = "8";
            rep.Setup(r => r.GetById(It.IsAny<int>())).Returns((User)null);

            //Act
            
            //Assert
            Assert.Throws<NullReferenceException>(() => _service.CheckIdentity(hashedToken, id));
            _unitOfWork.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.UserRepository, Times.Once);
            rep.Verify(r => r.GetById(It.IsAny<int>()), Times.Once);
            
        }

        [Test]
        public void ChechIdentityWithTwoParameters_Negative_BadId()
        {
            //Arrange
            string hashedToken = "pes";
            string id = "lol";
           
            //Act
            bool actual = _service.CheckIdentity(hashedToken, id);

            //Assert
            _unitOfWork.Verify(f => f.GetUnitOfWork(), Times.Once);
            Assert.IsFalse(actual);
        }

    }
}
