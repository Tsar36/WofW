using System.Collections.Generic;
using NUnit.Framework;
using WorldOfWords.Domain.Models;


namespace WorldOfWords.API.Models.Tests
{
    [TestFixture]
    class CourseMapperTests
    {
        [Test]
        public void Map_CourseAndCourseModelAreEqual()
        {
            
            //    var initial = new Course
            //    {
            //        Name = "English. A1",
            //        WordSuites = new List<WordSuite>
            //        {
            //            new WordSuite
            //            {
            //                Id = 1,
            //                Name = "Day of week"
            //            },
            //            new WordSuite
            //            {
            //                Id = 2,
            //                Name = "Family"
            //            },
            //            new WordSuite
            //            {
            //                Id = 3,
            //                Name = "Months"
            //            }  
            //        }
            //    };

            //    var expected = new CourseModel
            //    {
            //        Name = "English. A1",
            //        WordSuites = new List<CourseWordSuiteModel>
            //        {
            //            new CourseWordSuiteModel
            //            {
            //                Id = 1,
            //                Name = "Day of week"
            //            },
            //            new CourseWordSuiteModel
            //            {
            //                Id = 2,
            //                Name = "Family"
            //            },
            //            new CourseWordSuiteModel
            //            {
            //                Id = 3,
            //                Name = "Months"
            //            }  
            //        }
            //    };

            //    //Action
            //    Mock<IWordSuiteMapper> wordSuiteMapper = new Mock<IWordSuiteMapper>();
            //    CourseMapper mapper = new CourseMapper(wordSuiteMapper.Object);            
            //    wordSuiteMapper
            //        .Setup(x => x.Map(It.IsAny<List<WordSuite>>()))
            //        .Returns(expected.WordSuites);
            //    var actual = (mapper).Map(initial);           

            //    //Assert
            //    Assert.AreEqual(expected.Name, actual.Name);
            //    Assert.AreEqual(expected.WordSuites[0].Id, actual.WordSuites[0].Id);
            //    Assert.AreEqual(expected.WordSuites[1].Id, actual.WordSuites[1].Id);
            //    Assert.AreEqual(expected.WordSuites[2].Id, actual.WordSuites[2].Id);
            //    Assert.AreEqual(expected.WordSuites[0].Name, actual.WordSuites[0].Name);
            //    Assert.AreEqual(expected.WordSuites[1].Name, actual.WordSuites[1].Name);
            //    Assert.AreEqual(expected.WordSuites[2].Name, actual.WordSuites[2].Name);
            //}
        }
    }
}
