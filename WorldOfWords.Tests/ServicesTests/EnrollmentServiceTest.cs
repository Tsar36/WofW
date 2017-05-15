// Vorotniak Nazar
// Review: Natalia Dyrda

using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.Infrastructure.Data.EF.UnitOfWork;

namespace WorldOfWords.Tests.ServicesTests
{
    [TestFixture]
    class EnrollmentServiceTest
    {
        readonly Mock<IWorldOfWordsUow> unitOfWork = new Mock<IWorldOfWordsUow>();
        readonly Mock<IRepository<Enrollment>> enrollmentRepository = new Mock<IRepository<Enrollment>>();

        private EnrollmentService enrollmentService;

        [TestFixtureSetUp]
        public void Setup()
        {
            var unitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
            unitOfWorkFactory.Setup(x => x.GetUnitOfWork()).Returns(unitOfWork.Object);
            unitOfWork.Setup(x => x.EnrollmentRepository).Returns(enrollmentRepository.Object);

            enrollmentService = new EnrollmentService(unitOfWorkFactory.Object);
        }

        private IQueryable<Enrollment> GetTestEnrollmentList()
        {
            return new List<Enrollment>(new Enrollment[] {
                new Enrollment() { GroupId = 0, User = new User { Name = "B" }, Group = new Group()},
                new Enrollment() { GroupId = 1, User = new User { Name = "C" }, Group = new Group()},
                new Enrollment() { GroupId = 0, User = new User { Name = "A" }, Group = new Group()}
            }).AsQueryable();
        }

        [Test]
        public void GetByGroupIdTestPositive()
        {
            // Arrange
            enrollmentRepository.Setup(x => x.GetAll()).Returns(GetTestEnrollmentList());

            // Act
            List<Enrollment> l = enrollmentService.GetByGroupId(0);

            // Assert
            Assert.IsTrue(l.TrueForAll(x => x.GroupId == 0), "Should return only enrollments with passed groupId");
            Assert.AreEqual(2, l.Count, "Should return all enrollments with passed groupId");

            Assert.IsTrue(l[0].User.Name == "A", "Should return ordered by user name");
            Assert.IsTrue(l[1].User.Name == "B", "Should return ordered by user name");
        }

        [Test]
        public void GetByGroupIdTestNegative()
        {
            // Arrange
            enrollmentRepository.Setup(x => x.GetAll()).Returns(GetTestEnrollmentList());

            // Act
            List<Enrollment> l = enrollmentService.GetByGroupId(3);

            // Assert
            Assert.AreEqual(0, l.Count, "Should return empty enrollment list if there"
                + "aren't enrollments with passed groupId");
        }

        [Test]
        public void GetUsersNotBelongingToGroupTestPositive()
        {
            // Arrange

            // The group, not belonging to it users must be returned by test method
            var group = new Group
            {
                Id = 111,
                Course = new Course { Id = 222 },
                OwnerId = 333,
                CourseId = 222
            };

            var role1 = new Role { Name = "Student" };
            var role2 = new Role { Name = "AnotherRole" };

            // On the exit must be this user with this enrollment 

            // Enrollment to return
            var enrollment = new Enrollment
            {
                GroupId = 100,
                Group = new Group { Course = new Course { Id = 100 } }
            };

            // User to return
            var user = new User
            {
                Id = 100,
                Roles = new List<Role> { role1 },
                Enrollments = new List<Enrollment> { enrollment }
            };

            enrollment.User = user;

            // User to test "u.Id != group.OwnerId" condition in Service
            var user1 = new User
            {
                Id = 333,
                Roles = new List<Role> { role1 },
                Enrollments = new List<Enrollment> { enrollment }
            };

            // User to test "u.Enrollments.Select(e => e.Group.Course).FirstOrDefault(c => c.Id == course.Id) == null"
            // condition in Service
            var enrollment2 = new Enrollment
            {
                GroupId = 100,
                Group = new Group { Course = new Course { Id = 222 } }
            };
            var user2 = new User
            {
                Id = 333,
                Roles = new List<Role> { role1 },
                Enrollments = new List<Enrollment> { enrollment2 }
            };

            // User to test "!usersCurrentlyInGroup.Any(u2 => u2.Id == u.Id)" condition
            var user3 = new User
            {
                Id = 333,
                Roles = new List<Role> { role1 },
                Enrollments = new List<Enrollment> { enrollment2 }
            };

            // User to test "u.Roles.Select(r => r.Name).Contains("Student")" condition
            var user4 = new User
            {
                Id = 333,
                Roles = new List<Role> { role2 },
                Enrollments = new List<Enrollment> { enrollment2 }
            };

            // For enrollmentRepository to return
            var enrollments = new List<Enrollment>(new Enrollment[] {
                enrollment,
                new Enrollment { GroupId = 111, User = user1}
            }).AsQueryable();

            var users = (new List<User> { user, user1, user2, user3, user4 }).AsQueryable();

            var groupRepository = new Mock<IRepository<Group>>();
            unitOfWork.Setup(x => x.GroupRepository).Returns(groupRepository.Object);
            groupRepository.Setup(x => x.GetById(group.Id)).Returns(group);
            enrollmentRepository.Setup(x => x.GetAll()).Returns(enrollments);

            var userRepository = new Mock<IUserRepository>();
            unitOfWork.Setup(x => x.UserRepository).Returns(userRepository.Object);
            userRepository.Setup(x => x.GetAll()).Returns(users);

            // Act
            var result = enrollmentService.GetUsersNotBelongingToGroup(group.Id);

            // Assert
            Assert.IsTrue(result.TrueForAll(u => u.Roles.Select(r => r.Name).Contains("Student")),
                "User should have the Student role");
            Assert.IsTrue(result.TrueForAll(u => !u.Enrollments.Any( e => e.GroupId == group.Id)),
                "User shouldn't be in that group");
            Assert.IsTrue(result.TrueForAll(u => !u.Enrollments.Any(e => e.Group.CourseId == group.CourseId)),
                "User shouldn't be in the course that contains given group");
            Assert.IsTrue(result.TrueForAll(u => u.Id != group.OwnerId));
        }

        [Test]
        public void GetUsersNotBelongingToGroupTestNegative()
        {
            // Arrange
            var groupRepository = new Mock<IRepository<Group>>();
            unitOfWork.Setup(x => x.GroupRepository).Returns(groupRepository.Object);
            groupRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns<Group>(null);

            // Act
            var l = enrollmentService.GetUsersNotBelongingToGroup(It.IsAny<int>());

            // Assert
            Assert.IsNull(l, "Should return null if no group with given ID");
        }

        [Test]
        public void EnrollUsersToGroupTestPositive()
        {
            // Arrange
            var users = new List<User> {
                new User(), new User(), new User()
            };
            
            unitOfWork.ResetCalls();

            // Act
            enrollmentService.EnrollUsersToGroup(users, It.IsAny<int>());

            // Assert
            unitOfWork.Verify(x => x.Save(), Times.Once());
        }

        [Test]
        public void EnrollUsersToGroupTestNegative()
        {
            // Arrange
            unitOfWork.ResetCalls();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => enrollmentService.EnrollUsersToGroup(null, It.IsAny<int>()));
            unitOfWork.Verify(x => x.Save(), Times.Never());
        }

        [Test]
        public void DeleteByIdTest()
        {
            // Arrange
            unitOfWork.ResetCalls();

            // Act
            enrollmentService.DeleteById(It.IsAny<int>());

            // Assert
            enrollmentRepository.Verify(x => x.Delete(It.IsAny<int>()), Times.Once(), "Should delete once");
            unitOfWork.Verify(x => x.Save(), Times.Once(), "Should save once");
        }
    }
}
