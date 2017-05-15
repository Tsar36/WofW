using Moq;
using NUnit.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;
using WorldofWords.Controllers;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services;

namespace WorldOfWords.Tests.ControllersTests
{
    [TestFixture]
    public class EnrollmentControllerTest
    {
        EnrollmentController _controller;
        Mock<IEnrollmentMapper> _enrollmentMapper;
        Mock<ICourseService> _courseService;
        Mock<IEnrollmentService> _enrollmentService;
        Mock<IWordSuiteService> _wordSuiteService;
        Mock<IWordProgressService> _wordProgressService;
        Mock<IGroupService> _groupService;
        Mock<IUserForListingMapper> _userMapper;

        [SetUp]
        public void Setup()
        {
            GenerateData("1", new[] { "NoRoles" });
            _enrollmentMapper = new Mock<IEnrollmentMapper>();
            _courseService = new Mock<ICourseService>();
            _enrollmentService = new Mock<IEnrollmentService>();
            _wordSuiteService = new Mock<IWordSuiteService>();
            _wordProgressService = new Mock<IWordProgressService>();
            _groupService = new Mock<IGroupService>();
            _userMapper = new Mock<IUserForListingMapper>();

            _controller = new EnrollmentController(_enrollmentService.Object, _enrollmentMapper.Object,
                _wordSuiteService.Object, _wordProgressService.Object, _userMapper.Object, _courseService.Object, _groupService.Object);
        }

        [Test]
        public void Get_GetEnrollmentsByGroupId_ReturnsEnrollments_Positive()
        {
            int groupId = 1;
            int userId = 1;
            int courseId = 1;

            var initialEnrollments = GetEnrollments(groupId);
            var initialGroup = GetGroup(groupId, userId, courseId);
            var userProgressMapping = GetUserProgressMapping();
            var expected = GetExpectedEnrollments(initialEnrollments, userProgressMapping);
            initialEnrollments.Reverse();

            _enrollmentService.Setup(x => x.GetByGroupId(groupId)).Returns(initialEnrollments);
            _enrollmentMapper.Setup(x => x.Map(initialEnrollments)).Returns(
                initialEnrollments.Select(e => new EnrollmentModel
                {
                    Id = e.Id,
                    GroupId = e.GroupId,
                    User = new UserForListingModel { Id = e.User.Id, Name = e.User.Name },
                    Date = string.Format("{0:dd.MM.yyyy}", e.Date)
                }).ToList());
            _groupService.Setup(x => x.GetById(groupId, userId)).Returns(initialGroup);
            _courseService.Setup(x => x.GetProgress(courseId,
                It.Is<int>(id => initialEnrollments.FirstOrDefault(e => e.User.Id == id) != null)))
                .Returns<int, int>((cId, uId) => userProgressMapping[uId]);

            var actual = _controller.GetByGroupId(groupId);

            Assert.That(actual, Is.EquivalentTo(expected).Using(new EnrollmentWithProgressModelComparer()));
        }
        [Test]
        public void Get_GetEnrollmentsByGroupId_ReturnsNull_Negative()
        {
            int groupId = 1;
            int userId = 1;
            int courseId = 1;

            var initialEnrollments = GetEnrollments(groupId);
            Group initialGroup = null;
            var userProgressMapping = GetUserProgressMapping();
            var expected = GetExpectedEnrollments(initialEnrollments, userProgressMapping);
            initialEnrollments.Reverse();

            _enrollmentService.Setup(x => x.GetByGroupId(groupId)).Returns(initialEnrollments);
            _enrollmentMapper.Setup(x => x.Map(initialEnrollments)).Returns(
                initialEnrollments.Select(e => new EnrollmentModel
                {
                    Id = e.Id,
                    GroupId = e.GroupId,
                    User = new UserForListingModel { Id = e.User.Id, Name = e.User.Name },
                    Date = string.Format("{0:dd.MM.yyyy}", e.Date)
                }).ToList());
            _groupService.Setup(x => x.GetById(groupId, userId)).Returns(initialGroup);
            _courseService.Setup(x => x.GetProgress(courseId,
                It.Is<int>(id => initialEnrollments.FirstOrDefault(e => e.User.Id == id) != null)))
                .Returns<int, int>((cId, uId) => userProgressMapping[uId]);

            var actual = _controller.GetByGroupId(groupId);

            Assert.That(actual, Is.Null);
        }
        [Test]
        public void Get_GetUsersNotBelongingToGroups_ReturnsUsers_Positive()
        {
            var initialUsers = GetUsers();
            var expected = GetUserModels();

            int groupId = 1;

            _enrollmentService.Setup(x => x.GetUsersNotBelongingToGroup(groupId)).Returns(initialUsers);
            _userMapper.Setup(x => x.Map(initialUsers)).Returns(expected);

            var actual = _controller.GetUsersNotBelongingToGroup(groupId);

            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public async void Post_EnrollUsersToGroup_ReturnsOkResult_Positive()
        {
            var initialUsers = GetUserModels();
            var mappedUsers = GetUsers();

            int groupId = 1;
            UsersForEnrollmentModel data = new UsersForEnrollmentModel
            {
                GroupId = groupId,
                UserModels = initialUsers
            };

            _userMapper.Setup(x => x.Map(initialUsers)).Returns(mappedUsers);
            _enrollmentService.Setup(x => x.EnrollUsersToGroup(mappedUsers, groupId));
            _wordSuiteService.Setup(x => x.CopyWordsuitesForUsersByGroupAsync(mappedUsers, groupId)).Returns(Task.FromResult<object>(null));
            _wordProgressService.Setup(x => x.CopyProgressesForUsersInGroup(mappedUsers, groupId));

            var actual = await  _controller.EnrollUsersToGroup(data);

            Assert.IsInstanceOf(typeof(OkResult), actual);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void Post_EnrollUsersToGroup_ThrowsArgumentNullException_Negative()
        {
            UsersForEnrollmentModel model = null;
            await _controller.EnrollUsersToGroup(model);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async void Post_EnrollUsersToGroup_ThrowsArgumentException_Negative()
        {
            var initialUsers = GetUserModels();
            var mappedUsers = GetUsers();

            int groupId = 1;
            UsersForEnrollmentModel data = new UsersForEnrollmentModel
            {
                GroupId = groupId,
                UserModels = initialUsers
            };

            _userMapper.Setup(x => x.Map(initialUsers)).Returns(mappedUsers);
            _enrollmentService.Setup(x => x.EnrollUsersToGroup(mappedUsers, groupId));
            _wordSuiteService.Setup(x => x.CopyWordsuitesForUsersByGroupAsync(mappedUsers, groupId)).Returns(Task.FromResult<object>(null));;
            _wordProgressService.Setup(x => x.CopyProgressesForUsersInGroup(mappedUsers, groupId)).Throws<ArgumentException>();

            await _controller.EnrollUsersToGroup(data);
        }
        [Test]
        public void Delete_DeleteEntrollment_ReturnsOkResult_Positive()
        {
            int enrollmentId = 1;

            _wordProgressService.Setup(x => x.RemoveProgressesForEnrollment(enrollmentId));
            _wordSuiteService.Setup(x => x.RemoveWordSuitesForEnrollment(enrollmentId));
            _enrollmentService.Setup(x => x.DeleteById(enrollmentId));

            var actual = _controller.Delete(enrollmentId);

            Assert.IsInstanceOf(typeof(OkResult), actual);
        }
        [Test]
        public void Delete_DeleteEntrollment_ThrowsArgumentNullException_Negative()
        {
            int enrollmentId = 1;

            _wordProgressService.Setup(x => x.RemoveProgressesForEnrollment(enrollmentId)).Throws<ArgumentNullException>();
            _wordSuiteService.Setup(x => x.RemoveWordSuitesForEnrollment(enrollmentId));
            _enrollmentService.Setup(x => x.DeleteById(enrollmentId));

            Assert.Throws<ArgumentNullException>(() => _controller.Delete(enrollmentId));
        }

        private void GenerateData(string name, string[] roles)
        {
            GenericIdentity identity = new GenericIdentity(name);
            Thread.CurrentPrincipal = new GenericPrincipal(identity, roles);
        }

        private List<Enrollment> GetEnrollments(int groupId)
        {
            return new List<Enrollment>
            {
                new Enrollment
                {
                    Id = 1,
                    User = new User
                    {
                        Id = 1,
                        Name = "Roman"
                    },
                    GroupId = groupId,
                    Date = DateTime.Now
                },
                new Enrollment
                {
                    Id = 2,
                    User = new User
                    {
                        Id = 2,
                        Name = "Andriy"
                    },
                    GroupId = groupId,
                    Date = DateTime.Now
                },
                new Enrollment
                {
                    Id = 3,
                    User = new User
                    {
                        Id = 3,
                        Name = "Nazar"
                    },
                    GroupId = groupId,
                    Date = DateTime.Now
                }
            };
        }
        private List<User> GetUsers()
        {
            return new List<User>
            {
                new User
                {
                    Id = 4,
                    Name = "Sasha",
                    Email = "Sasha@example.com"                    
                },
                new User
                {
                    Id = 5,
                    Name = "Slava",
                    Email = "Slava@example.com"
                },
                new User
                {
                    Id = 6,
                    Name = "Yaryna",
                    Email = "Yaryna@example.com"
                },
                new User
                {
                    Id = 7,
                    Name = "Yura",
                    Email = "Yura@example.com"
                }
            };
        }
        private List<UserForListingModel> GetUserModels()
        {
            return new List<UserForListingModel>
            {
                new UserForListingModel
                {
                    Id = 4,
                    Name = "Sasha"
                },
                new UserForListingModel
                {
                    Id = 5,
                    Name = "Slava"
                },
                new UserForListingModel
                {
                    Id = 6,
                    Name = "Yaryna"
                },
                new UserForListingModel
                {
                    Id = 7,
                    Name = "Yura"
                }
            };
        }
        private Group GetGroup(int groupId, int userId, int courseId)
        {
            return new Group
            {
                Id = groupId,
                OwnerId = userId,
                Name = "Basic English",
                CourseId = courseId
            };
        }
        private Dictionary<int, double> GetUserProgressMapping()
        {
            var userProgressMapping = new Dictionary<int, double>();
            userProgressMapping.Add(1, 68.53);
            userProgressMapping.Add(2, 76.56);
            userProgressMapping.Add(3, 28.43);
            return userProgressMapping;
        }
        private List<EnrollmentWithProgressModel> GetExpectedEnrollments(List<Enrollment> initialEnrollments, 
            Dictionary<int, double> userProgressMapping)
        {
            var result = new List<EnrollmentWithProgressModel>();
            foreach (var enrollment in initialEnrollments)
            {
                result.Add(new EnrollmentWithProgressModel()
                {
                    Enrollment = new EnrollmentModel
                    {
                        Id = enrollment.Id,
                        User = new UserForListingModel
                        {
                            Id = enrollment.User.Id,
                            Name = enrollment.User.Name
                        },
                        GroupId = enrollment.GroupId,
                        Date = string.Format("{0:dd.MM.yyyy}", enrollment.Date)
                    },
                    Progress = userProgressMapping[enrollment.User.Id]
                });
            }
            return result;
        }
    }

    public class EnrollmentWithProgressModelComparer : IEqualityComparer<EnrollmentWithProgressModel>
    {
        public bool Equals(EnrollmentWithProgressModel x, EnrollmentWithProgressModel y)
        {
            return x.Enrollment.Id == y.Enrollment.Id && x.Enrollment.GroupId == y.Enrollment.GroupId
                && x.Enrollment.Date == y.Enrollment.Date && x.Enrollment.User.Id == y.Enrollment.User.Id
                && x.Enrollment.User.Name == y.Enrollment.User.Name && x.Progress == y.Progress;
        }

        public int GetHashCode(EnrollmentWithProgressModel obj)
        {
            if (obj != null)
            {
                return obj.Enrollment.Id.GetHashCode();
            }
            throw new ArgumentException("Parameter is not of valid type", "obj");
        }
    }
}
