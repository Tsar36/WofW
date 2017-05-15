using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using WorldofWords.Controllers;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services;

namespace WorldOfWords.Tests.ControllersTests
{
    [TestFixture]
    public class GroupControllerTest
    {
        GroupController _controller;
        Mock<IGroupForListingMapper> _groupForListingMapper;
        Mock<IGroupService> _groupService;
        Mock<IEnrollmentService> _enrollmentService;
        Mock<IWordProgressService> _wordProgressService;
        Mock<IWordSuiteService> _wordsuiteService;
        Mock<ICourseService> _courseService;
        Mock<ICourseForGroupMapper> _courseMapper;
        Mock<IGroupMapper> _groupMapper;

        [SetUp]
        public void Setup()
        {
            GenerateData("1", new[] { "NoRoles" });
            _groupForListingMapper = new Mock<IGroupForListingMapper>();
            _groupService = new Mock<IGroupService>();
            _enrollmentService = new Mock<IEnrollmentService>();
            _wordProgressService = new Mock<IWordProgressService>();
            _wordsuiteService = new Mock<IWordSuiteService>();
            _courseService = new Mock<ICourseService>();
            _courseMapper = new Mock<ICourseForGroupMapper>();
            _groupMapper = new Mock<IGroupMapper>();

            _controller = new GroupController(_groupService.Object, _groupForListingMapper.Object,
                _enrollmentService.Object, _wordProgressService.Object, _wordsuiteService.Object, _courseService.Object,
                _courseMapper.Object, _groupMapper.Object);
        }

        [Test]
        public void Get_Groups_ReturnsGroupsList_Positive()
        {
            var initial = GetInitialGroups();
            var expected = GetExpectedGroupModels();

            int userId = 1;
            
            _groupService.Setup(x => x.GetAll(userId)).Returns(initial);
            _groupForListingMapper.Setup(x => x.Map(initial)).Returns(expected);

            var actual = _controller.Get();

            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public void Get_GetGroupById_ReturnsNull_Negative()
        {
            Group expected = null;
            
            int userId = 1;
            int groupId = 111111;

            _groupService.Setup(x => x.GetById(groupId, userId)).Returns(expected);

            var actual = _controller.Get(groupId);

            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void Get_GetGroupById_ReturnsGroupModel_Positive()
        {
            var initial = GetInitialGroup();
            var expected = GetExpectedGroupModel();
                        
            int groupId = 1;
            int userId = 1;

            _groupService.Setup(x => x.GetById(groupId, userId)).Returns(initial);
            _groupForListingMapper.Setup(x => x.Map(initial)).Returns(expected);

            var actual = _controller.Get(groupId);

            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void Get_GetCourses_ReturnsCourses_Positive()
        {
            var initial = GetInitialCourses();
            var expected = GetExpectedCourseModels();

            int userId = 1;

            _courseService.Setup(x => x.GetAllCourses(userId)).Returns(initial);
            _courseMapper.Setup(x => x.Map(initial)).Returns(expected);

            var actual = _controller.GetCourses();
            
            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public void Post_NewGroup_ReturnsOkResult_Positive()
        {
            var initialModel = GetInitialGroupModel();
            var initial = GetInitialGroup();

            _groupService.Setup(x => x.ContainsGroupName(initialModel)).Returns(false);
            _groupService.Setup(x => x.Add(initial));

            var actual = _controller.Post(initialModel);

            Assert.IsInstanceOf(typeof(OkResult), actual);
        }
        [Test]
        public void Post_NewGroup_ThrownException_Negative()
        {
            GroupModel initial = null;

            Assert.Throws<ArgumentNullException>(() => _controller.Post(initial));
        }
        [Test]
        public void Post_NewGroup_ReturnsBadRequestResult_Negative()
        {
            var initialModel = GetInitialGroupModel();
            var initial = GetInitialGroup();

            _groupService.Setup(x => x.ContainsGroupName(initialModel)).Returns(true);
            _groupService.Setup(x => x.Add(initial));

            var actual = _controller.Post(initialModel);

            Assert.IsInstanceOf(typeof(BadRequestErrorMessageResult), actual);
        }
        [Test]
        public void Delete_Group_ReturnsOkResult_Positive()
        {
            int groupId = 1;
            List<Enrollment> initialEnrollments = GetInitialEnrollments(groupId);

            _enrollmentService.Setup(x => x.GetByGroupId(groupId)).Returns(initialEnrollments);
            _wordProgressService.Setup(x => x.RemoveProgressesForEnrollment(
                It.Is<int>(id => initialEnrollments.FirstOrDefault(e => e.Id == id) != null)));
            _wordsuiteService.Setup(x => x.RemoveWordSuitesForEnrollment(
                It.Is<int>(id => initialEnrollments.FirstOrDefault(e => e.Id == id) != null)));
            _groupService.Setup(x => x.DeleteById(groupId));

            var actual = _controller.Delete(groupId);

            Assert.IsInstanceOf(typeof(OkResult), actual);
        }
        [Test]
        public void Delete_Group_ThrowsArgumentNullException_Negative()
        {
            int groupId = 1;
            List<Enrollment> initialEnrollments = GetInitialEnrollments(groupId);

            _enrollmentService.Setup(x => x.GetByGroupId(groupId)).Returns(initialEnrollments);
            _wordProgressService.Setup(x => x.RemoveProgressesForEnrollment(
                It.Is<int>(id => initialEnrollments.FirstOrDefault(e => e.Id == id) != null)));
            _wordsuiteService.Setup(x => x.RemoveWordSuitesForEnrollment(
                It.Is<int>(id => initialEnrollments.FirstOrDefault(e => e.Id == id) != null)));
            _groupService.Setup(x => x.DeleteById(groupId)).Throws<ArgumentNullException>();

            Assert.Throws<ArgumentNullException>(() => _controller.Delete(groupId));
        }

        private void GenerateData(string name, string[] roles)
        {
            GenericIdentity identity = new GenericIdentity(name);
            Thread.CurrentPrincipal = new GenericPrincipal(identity, roles);
        }

        private List<Group> GetInitialGroups()
        {
            return new List<Group>
            {
                new Group
                {
                    Id=1,
                    CourseId=1,
                    OwnerId=1,
                    Name="Basic English",
                    Course = new Course
                    {
                        Name = "English.A1"
                    }
                },
                new Group
                {
                    Id=2,
                    CourseId=2,
                    OwnerId=1,
                    Name="Basic German",
                    Course = new Course
                    {
                        Name = "German.A1"
                    }
                },
                new Group
                {
                    Id=3,
                    CourseId=3,
                    OwnerId=1,
                    Name="Basic French",
                    Course = new Course
                    {
                        Name = "French.A1"
                    }
                }
            };
        }
        private Group GetInitialGroup()
        {
            return new Group
            {
                Id = 1,
                OwnerId = 1,
                CourseId = 1,
                Name = "Some Group Name",
                Course = new Course
                {
                    Name = "English.A1"
                }
            };
        }
        private GroupModel GetInitialGroupModel()
        {
            return new GroupModel
            {
                Name = "Some Group Name",
                OwnerId = 1,
                CourseId = 1
            };
        }
        private GroupForListingModel GetExpectedGroupModel()
        {
            return new GroupForListingModel
            {
                Id = 1,
                Name = "Some Group Name",
                CourseId = 1,
                CourseName = "English.A1"
            };
        }
        private List<GroupForListingModel> GetExpectedGroupModels()
        {
            return new List<GroupForListingModel>
            {
                new GroupForListingModel
                {
                    Id=1,
                    CourseId=1,
                    Name="Basic English",
                    CourseName = "English.A1"
                },
                new GroupForListingModel
                {
                    Id=2,
                    CourseId=2,
                    Name="Basic German",
                    CourseName="German.A1"
                },
                new GroupForListingModel
                {
                    Id=3,
                    CourseId=3,
                    Name="Basic French",
                    CourseName="French.A1"
                }
            };
        }
        private List<Course> GetInitialCourses()
        {
            return new List<Course>
            {
                new Course
                {
                    Id = 1,
                    Name = "English.A1",
                    LanguageId = 1,
                    OwnerId = 1,
                    IsPrivate = false
                },
                new Course
                {
                    Id = 2,
                    Name = "French.A1",
                    LanguageId = 2,
                    OwnerId = 1,
                    IsPrivate = false
                }
            };
        }
        private List<CourseForGroupModel> GetExpectedCourseModels()
        {
            return new List<CourseForGroupModel>
            {
                new CourseForGroupModel
                {
                    Id = 1,
                    Name = "English.A1"
                },
                new CourseForGroupModel
                {
                    Id = 2,
                    Name = "French.A1"
                }
            };
        }
        private List<Enrollment> GetInitialEnrollments(int groupId)
        {
            return new List<Enrollment>
            {
                new Enrollment
                {
                    Id = 1,
                    GroupId = groupId,
                    UserId = 1,
                    Date = DateTime.Now
                },
                new Enrollment
                {
                    Id = 2,
                    GroupId = groupId,
                    UserId = 2,
                    Date = DateTime.Now
                },
                new Enrollment
                {
                    Id = 3,
                    GroupId = groupId,
                    UserId = 3,
                    Date = DateTime.Now
                }
            };
        }
    }
}
