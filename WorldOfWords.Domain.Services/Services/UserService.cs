using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WorldOfWords.API.Models;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Domain.Services.MessagesAndConsts;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.Validation;

namespace WorldOfWords.Domain.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IIncomingUserMapper _incomingUserMapper;
        private readonly PasswordHasher _passwordHasher = new PasswordHasher();
        private readonly ITokenValidation _token;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IUserForListingMapper _userListMapper;

        public UserService(IIncomingUserMapper incomingUserMapper, ITokenValidation token, IUnitOfWorkFactory unitOfWorkFactory,
           IUserForListingMapper userListMapper)
        {
            _incomingUserMapper = incomingUserMapper;
            _token = token;
            _unitOfWorkFactory = unitOfWorkFactory;
            _userListMapper = userListMapper;
        }

        public bool VerifyPasswords(string userFromDb, string user)
        {
            return _passwordHasher.VerifyHashedPassword(userFromDb, user)
                   == PasswordVerificationResult.Success;
        }

        #region Checking User Authorization
        public bool CheckUserAuthorization(UserWithPasswordModel userLoginApi)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var user = uow.UserRepository.GetAll().FirstOrDefault(u => ((u.Email).ToLower() == (userLoginApi.Email).ToLower()));
                if ((user != null) && (VerifyPasswords(user.Password, userLoginApi.Password)))
                {
                    userLoginApi.Id = user.Id;
                    userLoginApi.Roles = user.Roles.Select(x => x.Name);
                    user.HashedToken = _token.Sha256Hash(_token.RandomString(ConstContainer.HashLength));
                    userLoginApi.HashToken = user.HashedToken;
                    uow.UserRepository.AddOrUpdate(user);
                    uow.Save();
                    return true;
                }
                return false;
            }
        }
        #endregion

        public void Add(IncomingUser user)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                uow.IncomingUserRepository.AddOrUpdate(user);
                uow.Save();
            }
        }

        public void AddToken(IncomingUser user)
        {
            if (Exists(user))
            {
                using (var uow = _unitOfWorkFactory.GetUnitOfWork())
                {
                    var searchedUser = uow.IncomingUserRepository.GetById(user.Id);
                    searchedUser.Token = user.Token;
                    uow.Save();
                }
            }
        }

        public bool ConfirmUserRegistration(int userId, string token)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                if (uow.IncomingUserRepository.GetAll().Any(user => user.Id == userId && user.Token == token))
                {
                    var targetUser =
                        uow.IncomingUserRepository.GetAll().First(user => user.Id == userId && user.Token == token);
                    uow.IncomingUserRepository.Delete(targetUser);
                    var newUser = _incomingUserMapper.ToDomainModel(targetUser);
                    newUser.Password = _passwordHasher.HashPassword(newUser.Password);
                    newUser.Roles.Add(uow.RoleRepository.GetAll().First(role => role.Name == "Student"));
                    uow.UserRepository.Add(newUser);
                    uow.Save();

                    return true;
                }
                return false;
            }
        }

        public bool Exists(IncomingUser user)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return
                    uow.UserRepository.GetAll().Any(users => users.Email.ToLower() == user.Email.ToLower())
                    || uow.IncomingUserRepository.GetAll().Any(users => users.Email.ToLower() == user.Email.ToLower());
            }
        }

        public bool CheckUserName(string checkName, int userId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var user = uow.UserRepository.GetById(userId);
                return (user != null) && (user.Name == checkName);
            }
        }

        public bool CheckUserPassword(string checkPassword, int userId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var user = uow.UserRepository.GetById(userId);
                return (user != null) && (VerifyPasswords(user.Password, checkPassword));
            }
        }

        public bool CheckUserEmail(ForgotPasswordUserModel checkEmail)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var user = uow.UserRepository.GetAll().FirstOrDefault(us => us.Email == checkEmail.Email);
                if (user != null)
                {
                    checkEmail.Id = user.Id.ToString();
                    return true;
                }
                return false;
            }
        }

        public bool EditUserName(string newName, int userId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var user = uow.UserRepository.GetById(userId);
                if (user != null)
                {
                    user.Name = newName;
                    uow.Save();
                    return true;
                }
                return false;
            }
        }

        public bool EditUserPassword(string newPassword, int userId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var user = uow.UserRepository.GetById(userId);
                if (user != null)
                {
                    user.Password = _passwordHasher.HashPassword(newPassword);
                    uow.Save();
                    return true;
                }
                return false;
            }
        }

        public string GetUserName(int userId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var user = uow.UserRepository.GetById(userId);
                if (user != null)
                {
                    return user.Name;
                }
            }
            return null;
        }

        public List<User> GetAllUsers()
        {
            List<User> result;
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                result = uow.UserRepository.GetAll().Include(u => u.Roles).ToList();
            }
            return result;
        }

        public int GetAmountOfUsersByRoleId(int roleId = 0)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return (roleId == 0) ? uow.UserRepository.GetAll().Include(item => item.Roles).Count()
                    : uow.UserRepository.GetAll().Include(item => item.Roles).Where(item => item.Roles.Any(local => local.Id == roleId)).Count();
            }
        }


        //mfomitc:
        public async Task<IEnumerable<UserForListingModel>> GetUsersByRoleIdAsync(int roleId)
        {
            List<User> userList;
            List<User> teacherList = new List<User>();
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {                
                userList = await uow.UserRepository.GetAll().ToListAsync();
                teacherList.AddRange(userList
                    .Where(item1 => item1.Roles.Any(local => local.Id == roleId))
                    .Select(w => new User
                    {
                        Id = w.Id,
                        Name = w.Name
                    }));
            }
            return teacherList.Select(t => _userListMapper.Map(t));
        }

        public List<User> GetUsersFromIntervalByRoleId(int startOfInterval, int endOfInterval, int roleId = 0)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                if (startOfInterval >= uow.UserRepository.GetAll().Count() || startOfInterval < 0
                    || startOfInterval > endOfInterval || endOfInterval > uow.UserRepository.GetAll().Count())
                    throw new ArgumentException("Start of interval is bigger than end");
                if (roleId != 0)
                {
                    return uow.UserRepository.GetAll()
                        .Include(item => item.Roles)
                        .OrderBy(item => item.Name)
                        .Where(item => item.Roles.Any(local => local.Id == roleId))
                        .Skip(startOfInterval)
                        .Take(endOfInterval - startOfInterval)
                        .ToList();
                }
                else
                {
                    return uow.UserRepository.GetAll()
                        .Include(item => item.Roles)
                        .OrderBy(item => item.Name)
                        .Skip(startOfInterval)
                        .Take(endOfInterval - startOfInterval)
                        .ToList();
                }

            }
        }

        public bool CheckUserId(ForgotPasswordUserModel checkEmail)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var users = uow.UserRepository.GetAll();
                foreach (var user in users)
                {
                    if (_token.GetHashSha256(user.Id.ToString()) == checkEmail.Id)
                    {
                        checkEmail.Id = user.Id.ToString();
                        return true;
                    }
                }
                return false;
            }
        }

        public bool ChangeRolesOfUser(User user)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                User userToChange = uow.UserRepository.GetAll().Include("Roles").FirstOrDefault(item => item.Id == user.Id);
                userToChange.Roles = new List<Role>();
                var queryOfNeededIds = user.Roles.Select(item => item.Id);
                var toAdd = uow.RoleRepository.GetAll().Where(item => queryOfNeededIds.Contains(item.Id)).ToList();
                foreach (var a in toAdd)
                {
                    userToChange.Roles.Add(a);
                }
                uow.Save();
            }
            return true;
        }

        public void ChangePassword(ForgotPasswordUserModel model)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var targetUser =
                        uow.UserRepository.GetAll().First(user => user.Id.ToString() == model.Id);
                targetUser.Password = _passwordHasher.HashPassword(model.Password);
                uow.Save();
            }
        }

        public string GetUserMailById(int id)
        {
            string email = string.Empty;

            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var user = uow.UserRepository.GetAll().FirstOrDefault(u => u.Id == id);
            }
            return email;
        }

        public List<User> SearchByNameAndRole(string name, int roleid = 0)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var neededUsers = uow.UserRepository.GetAll()
                    .Include("Roles")
                    .Where(item => item.Name.StartsWith(name));
                return (roleid == 0) ? neededUsers.ToList() : neededUsers.Where(item => item.Roles.Any(local => local.Id == roleid)).ToList();
            }
        }

        // Written by Andriy Kusyi
        // Review: 
        public IEnumerable<string> GetCoursesNamesByUserId(int userId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var enrolments = uow.CourseRepository.GetAll()
                    .SelectMany(c => c.Groups.
                        SelectMany(g => g.Enrollments.
                            Where(e => e.UserId == userId)));

                IEnumerable<string> courseNames = enrolments.Select(e => e.Group
                    .Course
                    .Name)
                    .Distinct()
                    .ToList();

                return courseNames;
            }
        }
    }
}
