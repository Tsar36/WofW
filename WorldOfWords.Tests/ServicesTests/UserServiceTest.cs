using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using WorldOfWords.API.Models;
using WorldOfWords.API.Models.Mappers;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Domain.Services.Services;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.Infrastructure.Data.EF.UnitOfWork;
using WorldOfWords.Validation;
using Microsoft.AspNet.Identity;
using System;

namespace WorldOfWords.Tests.ServicesTests
{
    //Done by Guchko Andrew
    [TestFixture]
    public class UserServiceTest
    {
        Mock<IUnitOfWorkFactory> uowFactory;
        Mock<IWorldOfWordsUow> worldOfWordsUow;
        Mock<IIncomingUserRepository> incUserReposit;
        Mock<ITokenValidation> tokenVaidation;
        Mock<IUserRepository> userReposit;
        Mock<IUserService> iusService;
        Mock<IIncomingUserMapper> incUserMapper;
        Mock<IRepository<WorldOfWords.Domain.Models.Role>> roleRepository;
        Mock<IUserForListingMapper> userForListMaper;


        IncomingUser entity;
        UserService usServ;

        [SetUp]
        public void Setup()
        {
            uowFactory = new Mock<IUnitOfWorkFactory>();
            worldOfWordsUow = new Mock<IWorldOfWordsUow>();
            incUserReposit = new Mock<IIncomingUserRepository>();
            tokenVaidation = new Mock<ITokenValidation>();
            userReposit = new Mock<IUserRepository>();
            iusService = new Mock<IUserService>();
            incUserMapper = new Mock<IIncomingUserMapper>();
            userForListMaper = new Mock<IUserForListingMapper>();
            uowFactory.Setup(t => t.GetUnitOfWork()).Returns(worldOfWordsUow.Object);
            usServ = new UserService(incUserMapper.Object, tokenVaidation.Object, uowFactory.Object, userForListMaper.Object);
            roleRepository = new Mock<IRepository<Role>>();

            entity = new IncomingUser
            {
                Email = "roman@example.com",
                Id = 1,
                Password = "3452",
                Name = "Roman",
                Token = "jsdjsjd"
            };
        }

        #region VerifyPassword
        [Test]//Work +
        public void VerifyPassword_IsTrue_Test()
        {
            User user = new User { Password = "asdasdasd" };
            //You can't mocked PasswordHasher becouse it's a class, not a interface
            //Mock<PasswordHasher> passHesher = new Mock<PasswordHasher>();
            // passHesher.Setup(t => t.VerifyHashedPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(PasswordVerificationResult.Success);
            //passHesher.Setup(t => t.HashPassword("asdasdasd")).Returns("D8A928B2043DB77E340B523547BF16CB4AA483F0645FE0A290ED1F20AAB76257");
            // passHesher.CallBase = true;
            PasswordHasher ph = new PasswordHasher();
            string userFromDb = ph.HashPassword(user.Password);

            var result = usServ.VerifyPasswords(userFromDb, user.Password);
            Assert.IsTrue(result);
        }

        [Test]//Work +
        public void VerifyPassword_IsFalse_Test()
        {
            User user = new User { Password = "asdasdasd" };
            //You can't mocked PasswordHasher becouse it's a class, not a interface
            //Mock<PasswordHasher> passHesher = new Mock<PasswordHasher>();
            // passHesher.Setup(t => t.VerifyHashedPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(PasswordVerificationResult.Success);
            //passHesher.Setup(t => t.HashPassword("asdasdasd")).Returns("D8A928B2043DB77E340B523547BF16CB4AA483F0645FE0A290ED1F20AAB76257");
            // passHesher.CallBase = true;
            PasswordHasher ph = new PasswordHasher();
            string userFromDb = ph.HashPassword("ajjkks");

            var result = usServ.VerifyPasswords(userFromDb, user.Password);
            Assert.IsFalse(result);
        }
        #endregion

        //Work +
        #region CheckUserAuthorization_Test
        [Test] //+
        public void CheckUserAuthorization_IsTrue_Test()
        {
            //Arrange
            UserWithPasswordModel userLoginApi = new UserWithPasswordModel();
            PasswordHasher passHesher = new PasswordHasher();

            User user = new User();
            user.Email = entity.Email;
            user.Id = 4;
            user.Roles = new List<Role> { new Role() { Name = "admin", Id = user.Id } };

            userLoginApi.Password = "hukolp565";
            userLoginApi.Email = user.Email;
            string userPassword = passHesher.HashPassword(userLoginApi.Password);
            user.Password = userPassword;

            IQueryable<User> Users = new List<User>
            {
                user
            }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(Users);
            userReposit.Setup(t => t.AddOrUpdate(It.IsAny<User>())).Verifiable("added or updated");
            worldOfWordsUow.Setup(t => t.Save()).Verifiable("Saved");

            //Act
            var result = usServ.CheckUserAuthorization(userLoginApi);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Exactly(2));
            userReposit.Verify(t => t.AddOrUpdate(It.IsAny<User>()), Times.Once);
            worldOfWordsUow.VerifyAll();
            Assert.IsTrue(result);
        }

        [Test] //+
        public void CheckUserAuthorization_UserIsNull_Test()
        {
            //Arrange
            UserWithPasswordModel userLoginApi = new UserWithPasswordModel();
            PasswordHasher passHesher = new PasswordHasher();

            User user = new User();
            user.Email = entity.Email;
            user.Id = 4;
            user.Roles = new List<Role> { new Role() { Name = "admin", Id = user.Id } };

            userLoginApi.Password = "hukolp565";
            userLoginApi.Email = "blablabla@mail.ru";
            string userPassword = passHesher.HashPassword(userLoginApi.Password);
            user.Password = userPassword;

            IQueryable<User> Users = new List<User>
            {
                user
            }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(Users);
            userReposit.Setup(t => t.AddOrUpdate(user)).Verifiable("added or updated");
            worldOfWordsUow.Setup(t => t.Save()).Verifiable("Saved");

            //Act
            var result = usServ.CheckUserAuthorization(userLoginApi);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.AddOrUpdate(It.IsAny<User>()), Times.Never);
            worldOfWordsUow.Verify(t => t.Save(), Times.Never);
            Assert.IsFalse(result);
        }

        [Test] //+
        public void CheckUserAuthorization_PasswordsNotEqual_Test()
        {
            //Arrange
            UserWithPasswordModel userLoginApi = new UserWithPasswordModel();
            PasswordHasher passHesher = new PasswordHasher();

            User user = new User();
            user.Email = entity.Email;
            user.Id = 4;
            user.Roles = new List<Role> { new Role() { Name = "admin", Id = user.Id } };

            userLoginApi.Password = "hukolp565";
            userLoginApi.Email = user.Email;
            string userPassword = passHesher.HashPassword("blablabla");
            user.Password = userPassword;

            IQueryable<User> Users = new List<User>
            {
                user
            }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(Users);
            userReposit.Setup(t => t.AddOrUpdate(user)).Verifiable("added or updated");
            worldOfWordsUow.Setup(t => t.Save()).Verifiable("Saved");

            //Act
            var result = usServ.CheckUserAuthorization(userLoginApi);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.AddOrUpdate(It.IsAny<User>()), Times.Never);
            worldOfWordsUow.Verify(t => t.Save(), Times.Never);
            Assert.IsFalse(result);
        }
        #endregion

        [Test] //Work - 100% +
        public void Add_Test()
        {
            worldOfWordsUow.Setup(t => t.IncomingUserRepository).Returns(incUserReposit.Object);
            incUserReposit.Setup(i => i.AddOrUpdate(entity));
            worldOfWordsUow.Setup(t => t.Save()).Verifiable("Save method doesn't work");

            // Act
            usServ.Add(entity);

            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Exactly(1));
            incUserReposit.Verify(t => t.AddOrUpdate(entity), Times.Exactly(1));
            worldOfWordsUow.Verify(t => t.Save(), Times.Exactly(1));
        }

        #region AddToken
        [Test] //Work - 100% +
        public void AddTokenIsTrue_Test()
        {
            worldOfWordsUow.Setup(t => t.IncomingUserRepository).Returns(incUserReposit.Object);

            IQueryable<IncomingUser> incUsers = new List<IncomingUser>
            {
                entity
            }.AsQueryable<IncomingUser>();

            IQueryable<User> Users = new List<User>
            {
                new User {Email = "user@example.com", Id = 2}
            }.AsQueryable<User>();

            incUserReposit.Setup(t => t.GetAll()).Returns(incUsers);
            incUserReposit.Setup(t => t.GetById(1)).Returns(entity);

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(Users);

            //Act
            usServ.AddToken(new IncomingUser { Id = 1, Email = "roman@example.com" });

            #region assert
            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Exactly(2));
            worldOfWordsUow.Verify(t => t.IncomingUserRepository, Times.Exactly(2));
            incUserReposit.Verify(t => t.GetById(entity.Id), Times.Exactly(1));
            worldOfWordsUow.Verify(t => t.Save(), Times.Exactly(1));
            worldOfWordsUow.Setup(t => t.Save()).Verifiable("Error, not saving");
            Assert.AreEqual(entity.Email, "roman@example.com");
            #endregion
        }
        #endregion

        #region ConfirmUserRegistration
        [Test] //
        public void ConfirmUserRegistration_IsTrueTest()
        {
            //Arrange
            IncomingUser inUser = new IncomingUser
            {
                Email = "Stepan@example.com",
                Id = 1,
                Password = "3452",
                Name = "Stepan",
                Token = "jsdjsjd"
            };

            IQueryable<IncomingUser> incomUsers = new List<IncomingUser>
                {
                    inUser
                }.AsQueryable<IncomingUser>();

            User user = new User
            {
                Email = "Stepan@example.com",
                Id = 1,
                Password = "3452",
                Name = "Stepan",
                HashedToken = "sknsdmsopp"
            };

            IQueryable<Role> roles = new List<Role>
                {
                    new Role(){ Id = 1, Name = "Student"}
                }.AsQueryable<Role>();

            worldOfWordsUow.Setup(t => t.IncomingUserRepository).Returns(incUserReposit.Object);
            incUserReposit.Setup(t => t.GetAll()).Returns(incomUsers);
            incUserReposit.Setup(s => s.Delete(It.IsAny<IncomingUser>())).Verifiable("deleted");

            incUserMapper.Setup(t => t.ToDomainModel(It.IsAny<IncomingUser>())).Returns(user);
            worldOfWordsUow.Setup(t => t.RoleRepository).Returns(roleRepository.Object);
            roleRepository.Setup(t => t.GetAll()).Returns(roles);
            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.Add(user)).Verifiable("Added user");
            worldOfWordsUow.Setup(t => t.Save()).Verifiable("Saved");

            //Act
            var result = usServ.ConfirmUserRegistration(1, "jsdjsjd");

            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.IncomingUserRepository, Times.Exactly(3));
            incUserReposit.Verify(t => t.GetAll(), Times.Exactly(2));
            incUserReposit.VerifyAll();
            incUserMapper.Verify(t => t.ToDomainModel(It.IsAny<IncomingUser>()), Times.Once);
            worldOfWordsUow.Verify(t => t.RoleRepository, Times.Once);
            roleRepository.Verify(t => t.GetAll(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.VerifyAll();
            worldOfWordsUow.VerifyAll();

            Assert.IsTrue(result);
        }

        [Test] //
        public void ConfirmUserRegistration_IsFalseTest()
        {
            //Arrange
            IncomingUser inUser = new IncomingUser
            {
                Email = "Stepan@example.com",
                Id = 1,
                Password = "3452",
                Name = "Stepan",
                Token = "jsdjsjd"
            };

            IQueryable<IncomingUser> incomUsers = new List<IncomingUser>
                {
                    inUser
                }.AsQueryable<IncomingUser>();

            User user = new User
            {
                Email = "Stepan@example.com",
                Id = 1,
                Password = "3452",
                Name = "Stepan",
                HashedToken = "sknsdmsopp"
            };

            IQueryable<Role> roles = new List<Role>
                {
                    new Role(){ Id = 1, Name = "Student"}
                }.AsQueryable<Role>();

            worldOfWordsUow.Setup(t => t.IncomingUserRepository).Returns(incUserReposit.Object);
            incUserReposit.Setup(t => t.GetAll()).Returns(incomUsers);
            incUserReposit.Setup(s => s.Delete(It.IsAny<IncomingUser>())).Verifiable("deleted");

            incUserMapper.Setup(t => t.ToDomainModel(It.IsAny<IncomingUser>())).Returns(user);
            worldOfWordsUow.Setup(t => t.RoleRepository).Returns(roleRepository.Object);
            roleRepository.Setup(t => t.GetAll()).Returns(roles);
            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.Add(user)).Verifiable("Added user");
            worldOfWordsUow.Setup(t => t.Save()).Verifiable("Saved");

            //Act
            var result = usServ.ConfirmUserRegistration(2, "jaflkasjdl");

            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.IncomingUserRepository, Times.Once);
            incUserReposit.Verify(t => t.GetAll(), Times.Once);
            incUserReposit.Verify(t => t.Delete(It.IsAny<IncomingUser>()), Times.Never);
            incUserMapper.Verify(t => t.ToDomainModel(It.IsAny<IncomingUser>()), Times.Never);
            worldOfWordsUow.Verify(t => t.RoleRepository, Times.Never);
            roleRepository.Verify(t => t.GetAll(), Times.Never);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Never);
            userReposit.Verify(t => t.Add(It.IsAny<User>()), Times.Never);
            worldOfWordsUow.Verify(t => t.Save(), Times.Never);

            Assert.IsFalse(result);
        }

        #endregion

        //Work - 100% +
        #region Exist
        [Test] //Work
        public void ExistIncoming_Test()
        {
            IQueryable<IncomingUser> incUsers = new List<IncomingUser>
            {
                entity
            }.AsQueryable<IncomingUser>();

            IQueryable<User> Users = new List<User>
            {
                new User {Email = "user@example.com", Id = 2}
            }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(Users);

            worldOfWordsUow.Setup(t => t.IncomingUserRepository).Returns(incUserReposit.Object);
            incUserReposit.Setup(t => t.GetAll()).Returns(incUsers);
            
            //Act
            var result = usServ.Exists(entity);
            
            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Exactly(1));
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Exactly(1));
            userReposit.Verify(t => t.GetAll(), Times.Exactly(1));

            worldOfWordsUow.Verify(t => t.IncomingUserRepository, Times.Exactly(1));
            incUserReposit.Verify(t => t.GetAll(), Times.Exactly(1));
            worldOfWordsUow.VerifyAll();

            Assert.AreEqual(incUsers.First().Email, entity.Email);
            Assert.IsTrue(result, "Variable is exist");
        }

        [Test] //Work
        public void ExistUser_Test()
        {
            //Arrange
            User user = new User();
            user.Email = entity.Email;
            IQueryable<User> Users = new List<User>
            {
                user
            }.AsQueryable();

            IQueryable<IncomingUser> incUsers = new List<IncomingUser>
            {
                entity
            }.AsQueryable<IncomingUser>();

            worldOfWordsUow.Setup(t => t.IncomingUserRepository).Returns(incUserReposit.Object);
            incUserReposit.Setup(t => t.GetAll()).Returns(incUsers);

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(Users);

            //Act
            var result = usServ.Exists(entity);

            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Exactly(1));
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Exactly(1));
            userReposit.Verify(t => t.GetAll(), Times.Exactly(1));

            worldOfWordsUow.Verify(t => t.IncomingUserRepository, Times.Exactly(0));
            incUserReposit.Verify(t => t.GetAll(), Times.Exactly(0));

            Assert.AreEqual(Users.First().Email, entity.Email);
            Assert.IsTrue(result, "Variable is exist");
        }
        #endregion

        //Work  - 100% +
        #region CheckUserName
        [Test] //+
        public void CheckUserNameIsTrue_Test()
        {
            //Arrange
            User user = new User();
            user.Name = "Petro";
            user.Id = 1;

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetById(It.IsAny<int>())).Returns(user);

            //Act
            var result = usServ.CheckUserName("Petro", 1);

            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetById(It.IsAny<int>()), Times.Once);
            Assert.IsTrue(result, "User is not Exist");
        }

        [Test] //+
        public void CheckUserNameIsFalse_UserIsNull_Test()
        {
            //Arrange
            User user = null;
            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetById(It.IsAny<int>())).Returns(user);
            
            //Act
            var result = usServ.CheckUserName("Andrii", 2);

            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetById(It.IsAny<int>()), Times.Once);
            Assert.IsFalse(result, "User is not Exist");
        }

        [Test] //+
        public void CheckUserNameIsFalse_NamesAreNotEqual_Test()
        {
            //Arrange
            User user = new User();
            user.Name = "Andrew";
            user.Id = 1;

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetById(It.IsAny<int>())).Returns(user);

            //Act
            var result = usServ.CheckUserName("Petro", 1);

            //Assert
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetById(It.IsAny<int>()), Times.Once);
            Assert.AreNotEqual(user.Name, "Petro");
            Assert.IsFalse(result, "User is not Exist");
        }

        #endregion

        //Work +
        #region CheckUserPassword
        [Test] //+
        public void CheckUserPasswordIsTrue_Test()
        {
            //Arrenge
            PasswordHasher passHesh = new PasswordHasher();
            string passsword = passHesh.HashPassword("1ah45k7");
            User user = new User();
            user.Name = "Stepan";
            user.Id = 3;
            user.Password = passsword;

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetById(It.IsAny<int>())).Returns(user);

            //Act
            var result = usServ.CheckUserPassword("1ah45k7", user.Id);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetById(It.IsAny<int>()), Times.Once);
            Assert.IsTrue(result);
        }

        [Test] //+
        public void CheckUserPasswordUserIsNull_Test()
        {
            //Arrenge
            PasswordHasher passHesh = new PasswordHasher();
            string passsword = passHesh.HashPassword("1ah45k7");
            User user = new User();
            user.Name = "Stepan";
            user.Id = 3;
            user.Password = passsword;

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetById(3)).Returns(user);

            //Act
            var result = usServ.CheckUserPassword("1ah45k7", 5);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetById(It.IsAny<int>()), Times.Once);
            Assert.IsFalse(result);
        }

        [Test] //+
        public void CheckUserPasswordPasswordNotEqual_Test()
        {
            //Arrange
            PasswordHasher passHesh = new PasswordHasher();
            string passsword = passHesh.HashPassword("1235sjkf");

            User user = new User();
            user.Name = "Stepan";
            user.Id = 3;
            user.Password = passsword;

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetById(It.IsAny<int>())).Returns(user);

            //Act
            var result = usServ.CheckUserPassword("1ah45k7", user.Id);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetById(It.IsAny<int>()), Times.Once);
            Assert.IsFalse(result);
        }
        #endregion

        //Work - 100% +
        #region CheckUserEmail
        [Test] //work +
        public void CheckUserEmailIsTrue_Test()
        {
            //Arrange
            User user = new User();
            user.Name = "Petro";
            user.Id = 1;
            user.Email = "roman@example.com";


            var forgotPassword = new ForgotPasswordUserModel();
            forgotPassword.Id = "5";
            forgotPassword.Email = "roman@example.com";

            entity.Id = 5;

            var reposit = new Mock<IRepository<IncomingUser>>();
            var iusReposit = new Mock<IUserRepository>();

            IQueryable<User> Users = new List<User>
            {
                user
            }.AsQueryable<User>();


            worldOfWordsUow.Setup(t => t.UserRepository).Returns(iusReposit.Object);
            iusReposit.Setup(t => t.GetAll()).Returns(Users);
            
            //Act
            var result = usServ.CheckUserEmail(forgotPassword);

            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Exactly(1));
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Exactly(1));
            iusReposit.Verify(t => t.GetAll(), Times.Exactly(1));
            Assert.IsTrue(result);
        }

        [Test]//work +
        public void CheckUserEmailIsFalse_Test()
        {
            //Arrange
            User user = new User();
            user.Name = "Petro";
            user.Id = 1;
            user.Email = "banan@example.com";


            var forgotPassword = new ForgotPasswordUserModel();
            forgotPassword.Id = "5";
            forgotPassword.Email = "roman@example.com";

            entity.Id = 5;

            var reposit = new Mock<IRepository<IncomingUser>>();
            var iusReposit = new Mock<IUserRepository>();

            IQueryable<User> Users = new List<User>
            {
                user
            }.AsQueryable<User>();


            worldOfWordsUow.Setup(t => t.UserRepository).Returns(iusReposit.Object);
            iusReposit.Setup(t => t.GetAll()).Returns(Users);

            //Act
            var result = usServ.CheckUserEmail(forgotPassword);

            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Exactly(1));
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Exactly(1));
            iusReposit.Verify(t => t.GetAll(), Times.Exactly(1));
            Assert.IsFalse(result);
        }
        #endregion

        //Work - 100% +
        #region EditUserName
        [Test] //Work + 
        public void EditUserNameIsNotNull_Test()
        {
            //Arrange
            User user = new User();
            user.Name = "Petro";
            user.Id = 1;

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetById(It.IsAny<int>())).Returns(user);
            worldOfWordsUow.Setup(t => t.Save()).Verifiable("All is fine");

            //Act
            var result = usServ.EditUserName("Andrew", 1);

            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Exactly(1));
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Exactly(1));
            userReposit.Verify(t => t.GetById(It.IsAny<int>()), Times.Exactly(1));
            worldOfWordsUow.VerifyAll();
            Assert.IsTrue(result);
            Assert.AreEqual(user.Name, "Andrew");
        }

        [Test] //+
        public void EditUserNameIsFalse_Test()
        {
            //Arrange
            User user = new User();
            user.Name = "Petro";
            user.Id = 1;
            user = null;

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetById(It.IsAny<int>())).Returns(user);
            worldOfWordsUow.Setup(t => t.Save()).Verifiable("All is fine");

            //Act
            var result = usServ.EditUserName("Andrew", 2);

            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Exactly(1));
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Exactly(1));
            userReposit.Verify(t => t.GetById(It.IsAny<int>()), Times.Exactly(1));
            worldOfWordsUow.Verify(t => t.Save(), Times.Never);
            Assert.IsNull(user);
            Assert.IsFalse(result);
        }

        #endregion

        //Work - 100% +
        #region EditUserPassword
        [Test] //work + 
        public void EditUserPasswordIsTrue_Test()
        {
            User user = new User();
            user.Email = "andrewguchko@mail.ru";
            user.Id = 1;
            user.Password = "bye258456";

            IQueryable<User> users = new List<User>()
            {
                user
            }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetById(It.IsAny<int>())).Returns(user);
            worldOfWordsUow.Setup(t => t.Save()).Verifiable("Saved");

            //Act
            var result = usServ.EditUserPassword("hello", 1);

            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Exactly(1));
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Exactly(1));
            userReposit.Verify(t => t.GetById(It.IsAny<int>()), Times.Exactly(1));
            worldOfWordsUow.VerifyAll();
            Assert.IsTrue(result);
        }

        [Test] //work +
        public void EditUserPasswordIsFalse_Test()
        {
            User user = new User();
            user.Email = "andrewguchko@mail.ru";
            user.Id = 1;
            user.Password = "bye258456";

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetById(1)).Returns(user);
            worldOfWordsUow.Setup(t => t.Save()).Verifiable("Saved");

            //Act
            var result = usServ.EditUserPassword("bye258456", 0);

            //Assert
            uowFactory.Verify(t => t.GetUnitOfWork(), Times.Exactly(1));
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Exactly(1));
            userReposit.Verify(t => t.GetById(0), Times.Exactly(1));
            Assert.IsFalse(result);

        }
        #endregion

        //Work - 100% +
        #region GetUserName
        [Test] //Work +
        public void GetUserNameIsNotNull_Test()
        {
            //Arrange
            User user = new User();
            user.Name = "Andrew";
            user.Id = 5;

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetById(It.IsAny<int>())).Returns(user);

            //Act
            var result = usServ.GetUserName(user.Id);
            
            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(f => f.UserRepository, Times.Once);
            userReposit.Verify(w => w.GetById(It.IsAny<int>()), Times.Once);
            Assert.IsNotNull(result);
        }

        [Test] //Work +
        public void GetUserNameIsNull_Test()
        {
            User user = new User();
            user.Name = "Andrew";
            user.Id = 5;

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetById(5)).Returns(user);

            var result = usServ.GetUserName(0);
            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(f => f.UserRepository, Times.Once);
            userReposit.Verify(w => w.GetById(5), Times.Never);
            userReposit.Verify(w => w.GetById(0), Times.Once);
            Assert.IsNull(result);
        }

        #endregion

        [Test] //work - 100% +
        public void GetAllUsers_Test()
        {
            //Arrange
            IQueryable<User> listOfUsers = new List<User>
              {
                new User{Id = 5, Name = "Andrew"},
                new User()
              }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(listOfUsers);

            //Act
            var result = usServ.GetAllUsers();

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(w => w.GetAll(), Times.Once);
            CollectionAssert.AreEqual(result, listOfUsers);
            Assert.IsNotNull(result);
        }

        //Work - 100% +
        #region GetAmountOfUsersByRoleId
        [Test] //work + 
        public void GetAmountOfUsersByRoleIdZero_Test()
        {
            //Arrange
            Role admin = new Role();
            admin.Id = 0;

            IQueryable<Role> rol = new List<Role>()
            {
                admin
            }.AsQueryable<Role>();

            User user = new User { Id = 5, Name = "Andrew" };
            var user_2 = new User();
            user.Roles = new List<Role>() { admin };
            user_2.Roles = new List<Role>() { admin };

            IQueryable<User> listOfUsers = new List<User>
           {
            user,
            user_2
           }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(listOfUsers);

            //Act
            int result = usServ.GetAmountOfUsersByRoleId();

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetAll(), Times.Once);
            Assert.IsTrue(result == 2);
        }

        [Test] //+
        public void GetAmountOfUsersByRoleIdNotZero_Test()
        {
            //Arrange
            Role admin = new Role();
            admin.Id = 1;

            IQueryable<Role> rol = new List<Role>()
            {
                admin
            }.AsQueryable<Role>();

            User user = new User { Id = 5, Name = "Andrew" };
            var user_2 = new User();
            user_2.Id = 3;
            user.Roles = new List<Role>() { admin };
            user_2.Roles = new List<Role>() { admin };

            IQueryable<User> listOfUsers = new List<User>
           {
            user,
            user_2
           }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(listOfUsers);

            //Act
            int result = usServ.GetAmountOfUsersByRoleId(1);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.AtLeastOnce());
            userReposit.Verify(t => t.GetAll(), Times.AtLeastOnce());

            Assert.IsTrue(result == 2);
        }
        #endregion

        //Work - 100% +
        #region GetUsersFromIntervalByRoleId
        [Test] //work  +
        //старт інтервал має починатись з 0, як і елементи колекції
        public void GetUsersFromIntervalByRoleId_IsRoleNotZero_Test()
        {
            //Arrange
            Role admin = new Role();
            admin.Id = 1;

            IQueryable<Role> rol = new List<Role>()
            {
                admin
            }.AsQueryable<Role>();

            IQueryable<User> listOfUsers = new List<User>
                            {
                                new User{Id = 5, Name = "Andrew", Roles = new List<Role>(){admin}}, 
                                new User{Id = 3, Name = "Fred", Roles = new List<Role>(){admin}},
                                new User{Id = 1, Name = "Sasha", Roles = new List<Role>(){admin}}

                            }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(listOfUsers);

            //Act
            var result = usServ.GetUsersFromIntervalByRoleId(0, 3, 1);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Exactly(3));
            userReposit.Verify(t => t.GetAll(), Times.Exactly(3));
            Assert.AreEqual(result, listOfUsers.ToList<User>());

        }

        [Test] //work +
        //старт інтервал має починатись з 0, як і елементи колекції
        public void GetUsersFromIntervalByRoleId_IsRoleZero_Test()
        {
            //Arrange
            Role admin = new Role();
            admin.Id = 0;

            IQueryable<Role> rol = new List<Role>()
            {
                admin
            }.AsQueryable<Role>();

            IQueryable<User> listOfUsers = new List<User>
                            {
                                new User{Id = 5, Name = "Andrew", Roles = new List<Role>(){admin}}, 
                                new User{Id = 3, Name = "Fred", Roles = new List<Role>(){admin}},
                                new User{Id = 1, Name = "Sasha", Roles = new List<Role>(){admin}}

                            }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(listOfUsers);

            //Act
            var result = usServ.GetUsersFromIntervalByRoleId(0, 3);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Exactly(3));
            userReposit.Verify(t => t.GetAll(), Times.Exactly(3));
            Assert.AreEqual(result, listOfUsers.ToList<User>());

        }

        [Test] //+
        public void GetUsersFromIntervalByRoleId_Exception_Test()
        {
            //Arrange
            Role admin = new Role();
            admin.Id = 0;

            IQueryable<Role> rol = new List<Role>()
            {
                admin
            }.AsQueryable<Role>();

            IQueryable<User> listOfUsers = new List<User>
                            {
                                new User{Id = 5, Name = "Andrew", Roles = new List<Role>(){admin}}, 
                                new User{Id = 3, Name = "Fred", Roles = new List<Role>(){admin}},
                                new User{Id = 1, Name = "Sasha", Roles = new List<Role>(){admin}}

                            }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(listOfUsers);

            //Act
            Assert.Throws<ArgumentException>(() => usServ.GetUsersFromIntervalByRoleId(-5, 0), "Start of interval is bigger than end");
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetAll(), Times.Once);
        }
        #endregion

        //Work
        #region CheckUserId
        [Test] //+
        public void CheckUserIdIsTrue_Test()
        {
            //Assert
            User user = new User { Email = "hgkjhllkj@mail.ru", Id = 15689 };
            ForgotPasswordUserModel checkEmail = new ForgotPasswordUserModel() { Id = user.Id.ToString(), Email = "andrewguchko@mail.ru" };

            IQueryable<User> users = new List<User>()
            {
                user
            }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(users);
            tokenVaidation.Setup(t => t.GetHashSha256(It.IsAny<string>())).Returns(checkEmail.Id);

            //Act
            var result = usServ.CheckUserId(checkEmail);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetAll(), Times.Once);
            tokenVaidation.Verify(t => t.GetHashSha256(user.Id.ToString()), Times.Once);
            Assert.IsTrue(result);
        }

        [Test] //+
        public void CheckUserIdIsFalse_Test()
        {
            //Assert
            User user = new User { Email = "hgkjhllkj@mail.ru", Id = 15689 };
            ForgotPasswordUserModel checkEmail = new ForgotPasswordUserModel() { Id = user.Id.ToString(), Email = "andrewguchko@mail.ru" };

            IQueryable<User> users = new List<User>()
            {
                user
            }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(users);
            tokenVaidation.Setup(t => t.GetHashSha256(It.IsAny<string>())).Returns("Hrin");

            //Act
            var result = usServ.CheckUserId(checkEmail);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetAll(), Times.Once);
            tokenVaidation.Verify(t => t.GetHashSha256(user.Id.ToString()), Times.Once);
            Assert.IsFalse(result);
        }

        #endregion


        [Test] //work - 100% +
        public void ChangeRolesOfUser_Test()
        {
            ///Arrange
            Mock<IRepository<WorldOfWords.Domain.Models.Role>> roleRepository = new Mock<IRepository<Role>>();

            Role admin = new Role();
            admin.Id = 1;

            IQueryable<Role> rol = new List<Role>()
            {
                admin
            }.AsQueryable<Role>();

            User user_2 = new User()
            {
                Email = "andrewguchko@mail.ru",
                Id = 1,
                Roles = new List<Role> 
                { 
                    new Role
                    {
                        Id = 1
                    }
                }
            };

            IQueryable<User> users = new List<User>()
            {
                new User
                {
                Email = "andrewguchko@mail.ru",
                Id = 1,
                Roles = new List<Role> 
                { 
                    new Role
                    {
                        Id = 1
                    }
                }}
            }.AsQueryable<User>();

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(users);
            worldOfWordsUow.Setup(t => t.RoleRepository).Returns(roleRepository.Object);
            roleRepository.Setup(t => t.GetAll()).Returns(rol);

            worldOfWordsUow.Setup(t => t.Save()).Verifiable();

            //Act
            var actual = usServ.ChangeRolesOfUser(user_2);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetAll(), Times.Once);
            worldOfWordsUow.Verify(t => t.RoleRepository, Times.Once);
            roleRepository.Verify(t => t.GetAll(), Times.Once);
            worldOfWordsUow.VerifyAll();
            Assert.IsTrue(actual);
        }

        [Test] //Work +
        public void ChangePassword_Test()
        {
            //Arrange
            //PasswordHasher pasHesh = new PasswordHasher();
            //string password = pasHesh.HashPassword("hjkol68");
            ForgotPasswordUserModel model = new ForgotPasswordUserModel { Id = "5", Password = "hjkol68" };
            User user = new User();
            user.Email = "andrewguchko@mail.ru";
            user.Id = 5;
            user.Password = "hjkol68";
            IQueryable<User> users = new List<User>()
            {
                user
            }.AsQueryable<User>();

            uowFactory.Setup(t => t.GetUnitOfWork()).Returns(worldOfWordsUow.Object);
            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(users);

            //Act
            usServ.ChangePassword(model);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetAll(), Times.Once);
            Assert.AreNotEqual(model.Password, user.Password);
            Assert.AreEqual(user.Id.ToString(), model.Id);
        }

        //Work - 100% +
        #region SearchByNameAndRole
        [Test] //work +
        public void SearchByNameAndRole_RoleIsNotZero_Test()
        {
            //Arrange
            IRepository<WorldOfWords.Domain.Models.Role> RoleReposit = new Mock<IRepository<WorldOfWords.Domain.Models.Role>>().Object;

            #region user and role
            User user = new User();
            user.Name = "Andrew";
            user.Email = "andrewguchko@mail.ru";
            user.Id = 2;

            Role admin = new Role();
            admin.Id = 1;
            user.Roles = new List<Role>() { admin };
            IQueryable<User> users = new List<User>()
            {
                user
            }.AsQueryable<User>();
            #endregion

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(users);

            //Act
            var userColl = usServ.SearchByNameAndRole("Andrew", 1);

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetAll(), Times.Once);
            Assert.IsNotNull(userColl);
        }

        [Test] //+
        public void SearchByNameAndRole_RoleIsZero_Test()
        {
            IRepository<WorldOfWords.Domain.Models.Role> RoleReposit = new Mock<IRepository<WorldOfWords.Domain.Models.Role>>().Object;

            #region user and role
            User user = new User();
            user.Name = "Andrew";
            user.Email = "andrewguchko@mail.ru";
            user.Id = 2;

            Role admin = new Role();
            admin.Id = 0;

            user.Roles = new List<Role>() { admin };

            IQueryable<User> users = new List<User>()
            {
                user
            }.AsQueryable<User>();
            #endregion

            worldOfWordsUow.Setup(t => t.UserRepository).Returns(userReposit.Object);
            userReposit.Setup(t => t.GetAll()).Returns(users);

            //Act
            var userColl = usServ.SearchByNameAndRole("Andrew");

            //Assert
            uowFactory.Verify(f => f.GetUnitOfWork(), Times.Once);
            worldOfWordsUow.Verify(t => t.UserRepository, Times.Once);
            userReposit.Verify(t => t.GetAll(), Times.Once);
            Assert.IsNotNull(userColl);
        }

        #endregion
    }
}


