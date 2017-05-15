//using System.Web.Http;
//using System.Web.Http.Results;
//using NUnit.Framework;
//using System.Collections.Generic;
//using WorldofWords.Controllers;
//using WorldOfWords.API.Models.IMappers;
//using WorldOfWords.API.Models;
//using WorldOfWords.Domain.Services;
//using WorldOfWords.Domain.Models;
//using Moq;
//using WorldOfWords.Domain.Services.IServices;
//using System.Security.Principal;
//using System.Threading;

//namespace WorldOfWords.Tests.ControllersTests
//{
//    [TestFixture]
//    class CourseControllerTest
//    {
//        private void GenerateData(string name, string[] roles)
//        {
//            GenericIdentity identity = new GenericIdentity(name);
//            Thread.CurrentPrincipal =
//                new GenericPrincipal(
//                    identity,
//                    roles
//                    );
//        }

//         [Test]
//        public void CourseController_GetEnrollCourses()
//        {
//            //Arrange
//            var initial = new List<Course>() 
//            {
//                new Course { Name = "English. A1" },
//                new Course { Name = "German. A1" },
//                new Course { Name = "France. A1" }
//            };

//            var expected = new List<CourseModel>() 
//            {
//                new CourseModel { Name = "English. A1" },
//                new CourseModel { Name = "German. A1" },
//                new CourseModel { Name = "France. A1" }
//            };
         
 
//            //Action
//             Mock<ICourseMapper> courseMapper = new Mock<ICourseMapper>();
//             Mock<ICourseService> courseService = new Mock<ICourseService>();
//             Mock<IWordSuiteService> wordSuiteService = new Mock<IWordSuiteService>();
//             GenerateData("1", new[] { "NoRoles" });
//             CourseController courseController = new CourseController(courseMapper.Object, courseService.Object, wordSuiteService.Object);
//             courseService.Setup(x=>x.GetEnrollCourses(It.IsAny<int>())).Returns(initial);
//             courseMapper
//                   .Setup(x => x.Map(initial))
//                    .Returns(expected);
//                var actual = courseController.GetEnrollCourses();

//            //Assert
//            Assert.AreEqual(expected.Count, actual.Count);


//        }

//        [Test]
//         public void CourseController_GetUserCourses()
//         {
//             //Arrange
//             var initial = new List<Course>() 
//            {
//                new Course { Name = "English. A1" },
//                new Course { Name = "German. A1" },
//                new Course { Name = "France. A1" }
//            };

//             var expected = new List<CourseModel>() 
//            {
//                new CourseModel { Name = "English. A1" },
//                new CourseModel { Name = "German. A1" },
//                new CourseModel { Name = "France. A1" }
//            };

//             //Action
//             Mock<ICourseMapper> courseMapper = new Mock<ICourseMapper>();
//             Mock<ICourseService> courseService = new Mock<ICourseService>();
//             Mock<IWordSuiteService> wordSuiteService = new Mock<IWordSuiteService>();
//             GenerateData("1", new[] { "NoRoles" });
//             CourseController courseController = new CourseController(courseMapper.Object, courseService.Object, wordSuiteService.Object);
//             courseService.Setup(x => x.GetUserCourses(It.IsAny<int>())).Returns(initial);
//             courseMapper
//                   .Setup(x => x.Map(initial))
//                    .Returns(expected);
//             var actual = courseController.GetUserCourses();

//             //Assert
//             Assert.AreEqual(expected.Count, actual.Count);
//         }

//        [Test]
//        public void CourseController_GetCourseProgress()
//        {
//            //Arrange
//            var initial = new Course()
//            {
//                Name = "English. A1"
//            };

//            var expected = new CourseModel()
//            {
//                Name = "English. A1",
//                Progress = 50,
//                WordSuites = new List<CourseWordSuiteModel>() 
//                { 
//                    new CourseWordSuiteModel()
//                    { Name = "Days of week"},
//                    new CourseWordSuiteModel()
//                    { Name = "Family member"},
//                    new CourseWordSuiteModel()
//                    { Name = "Seasons"}
//                }
//            };

//            //Action
//            Mock<ICourseMapper> courseMapper = new Mock<ICourseMapper>();
//            Mock<ICourseService> courseService = new Mock<ICourseService>();
//            Mock<IWordSuiteService> wordSuiteService = new Mock<IWordSuiteService>();
//            GenerateData("1", new[] { "NoRoles" });
//            CourseController courseController = new CourseController(courseMapper.Object, courseService.Object, wordSuiteService.Object);
//            courseService.Setup(x => x.GetById(It.IsAny<int>(), It.IsAny<int>())).Returns(initial);
//            courseService.Setup(x => x.GetProgress(It.IsAny<int>(), It.IsAny<int>())).Returns(50);
//            courseMapper
//                  .Setup(x => x.Map(initial))
//                   .Returns(expected);
//            wordSuiteService.Setup(x => x.GetWordSuiteProgress(It.IsAny<int>())).Returns(50);
//            var actual = courseController.GetCourseProgress(1);

//            //Assert
//            Assert.AreEqual(expected.Progress, actual.Progress);

//        }

//        //public void CourseController_Get()
//        //{
//        //    //Action
//        //    Mock<ICourseMapper> courseMapper = new Mock<ICourseMapper>();
//        //    Mock<ICourseService> courseService = new Mock<ICourseService>();
//        //    Mock<IWordSuiteService> wordSuiteService = new Mock<IWordSuiteService>();
//        //    CourseController courseController = new CourseController(courseMapper.Object, courseService.Object, wordSuiteService.Object);
//        //    courseService.Setup(x => x.Delete(It.IsAny<int>()));
//        //    var actual = courseController.Delete(1);
//        //    //Assert
//        //    Assert.AreEqual(typeof(OkResult), actual);

//        //}
//    }
//}
