// Nazar Vorotniak
// Review: not reviewed yet

using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.Infrastructure.Data.EF.UnitOfWork;

namespace WorldOfWords.Tests.ServicesTests
{
    [TestFixture]
    class GroupServiceTest
    {
        readonly Mock<IWorldOfWordsUow> unitOfWork = new Mock<IWorldOfWordsUow>();
        readonly Mock<IUnitOfWorkFactory> unitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        readonly Mock<IRepository<Group>> groupRepository = new Mock<IRepository<Group>>();
        private GroupService groupService;
        private IQueryable<Group> groups;

        [TestFixtureSetUp]
        public void Setup()
        {
            unitOfWorkFactory.Setup(x => x.GetUnitOfWork()).Returns(unitOfWork.Object);
            unitOfWork.Setup(x => x.GroupRepository).Returns(groupRepository.Object);
           
            groupService = new GroupService(unitOfWorkFactory.Object);

            var course1 = new Course { Id = 555 };
            var course2 = new Course { Id = 1 };

            groups = (new List<Group> { 
            new Group { Id = 111, OwnerId = 555, Course = course1, CourseId = 555, Name = "Name1" },
            new Group { Id = 222, OwnerId = 1, Course = course2, CourseId = 1, Name = "Name2" }}).AsQueryable();
        }

        [Test]
        public void GetAllTestPositive()
        {
            // Arrange
            groupRepository.Setup(x => x.GetAll()).Returns(groups);

            // Act
            var result = groupService.GetAll(555);

            // Assert
            Assert.AreEqual(1, result.Count, "Should return only groups that are owned by user");
            Assert.AreEqual(555, result[0].OwnerId, "Group must have id of owner");
            Assert.IsNotNull(result[0].Course, "Course must be included");
            Assert.IsTrue((result[0].CourseId == 555) && (result[0].Course.Id == 555), "Must be right relation by foreign key");
        }

        [Test]
        public void GetAllTestNegative()
        {
            // Arrange
            groupRepository.Setup(x => x.GetAll()).Returns(groups);

            // Act
            var result = groupService.GetAll(0);

            // Assert
            Assert.IsEmpty(result, "Should return empty collection if user hasn't any group");
        }

        [Test]
        public void ContainsGroupNameTestPositive()
        {
            // Arrange
            groupRepository.Setup(x => x.GetAll()).Returns(groups);

            var groupModel = new GroupModel { Name = "Name1" };

            // Act
            bool result1 = groupService.ContainsGroupName(groupModel);

            // Assert
            Assert.IsTrue(result1, "Should return true if a group exists with a such name");
        }

        [Test]
        public void ContainsGroupNameTestNegative()
        {
            // Arrange
            groupRepository.Setup(x => x.GetAll()).Returns(groups);

            var groupModel = new GroupModel { Name = "Name that doesn't exist" };

            // Act
            bool result1 = groupService.ContainsGroupName(groupModel);

            // Assert
            Assert.IsFalse(result1, "Should return false if a group doesn't exist with a such name");
        }

        [Test]
        public void AddTest()
        {
            // Arrange
            Group group = new Group();

            // To be sure that calls from another test methods don't calculate
            groupRepository.ResetCalls();
            unitOfWork.ResetCalls();

            // Act
            groupService.Add(group);

            // Assert
            groupRepository.Verify(x => x.Add(group), Times.Once());
            unitOfWork.Verify(x => x.Save(), Times.Once());
        }

        [Test]
        public void GetByIdTest()
        {
            // Arrange
            groupRepository.Setup(x => x.GetAll()).Returns(groups);

            // Act
            var result = groupService.GetById(111, 555);

            // Assert
            Assert.AreEqual(111, result.Id);
            Assert.AreEqual(555, result.OwnerId);

        }

        //[Test]
        //public void DeleteByIdTest()
        //{
        //    // Arrange
        //    int groupId = 555;
        //    var enrollments = (new List<Enrollment> { new Enrollment { GroupId = groupId } }).AsQueryable();

        //    var enrollmentRepository = new Mock<IRepository<Enrollment>>();
        //    unitOfWork.Setup(x => x.EnrollmentRepository).Returns(enrollmentRepository.Object);
        //    enrollmentRepository.Setup(x => x.GetAll()).Returns(enrollments);

        //    groupRepository.ResetCalls();

        //    // Act
        //    groupService.DeleteById(groupId);

        //    //Assert
        //    groupRepository.Verify(x => x.Delete(groupId), Times.Once());
        //    unitOfWork.Verify(x => x.Save(), Times.Once);
        //}
    }
}
