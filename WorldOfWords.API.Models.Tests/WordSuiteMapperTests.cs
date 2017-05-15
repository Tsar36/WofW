using System.Collections.Generic;
using NUnit.Framework;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models.Tests
{
    [TestFixture]
    class WordSuiteMapperTests
    {
        [Test]
        public void Map_WordSuiteAndCourseWordSuiteModelAreEqual()
        {            
            //Arrange
            var initial = new WordSuite
            {
                Id = 1,
                Name = "Day of week"
            };
            var expected = new CourseWordSuiteModel
            {
                Id = 1,
                Name = "Day of week"
            };
            //Act
            var actual = (new WordSuiteMapper()).Map(initial);

            //Assert
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
        }

        [Test]
        public void Map_WordSuitesAndCourseWordSuiteModelsAreEqual()
        {
            //Arrange
            List<WordSuite> initial = new List<WordSuite>
            {
                new WordSuite
                {
                    Id = 1,
                    Name = "Day of week"
                },
                new WordSuite
                {
                    Id = 2,
                    Name = "Family"
                },
                new WordSuite
                {
                    Id = 3,
                    Name = "Months"
                }               
            };
            List<CourseWordSuiteModel> expected = new List<CourseWordSuiteModel>
            {
                new CourseWordSuiteModel
                {
                    Id = 1,
                    Name = "Day of week"
                },
                new CourseWordSuiteModel
                {
                    Id = 2,
                    Name = "Family"
                },
                new CourseWordSuiteModel
                {
                    Id = 3,
                    Name = "Months"
                } 
            };

            //Act
            List<CourseWordSuiteModel> actual = (new WordSuiteMapper()).Map(initial);

            //Assert
            Assert.AreEqual(expected[0].Id, actual[0].Id);
            Assert.AreEqual(expected[1].Id, actual[1].Id);
            Assert.AreEqual(expected[2].Id, actual[2].Id);
            Assert.AreEqual(expected[0].Name, actual[0].Name);
            Assert.AreEqual(expected[1].Name, actual[1].Name);
            Assert.AreEqual(expected[2].Name, actual[2].Name);
        }

        [Test]
        public void MapForEditTest()
        {
            // Arrange
            WordSuite initial = new WordSuite
            {
                Id = 100,
                Name = "WordSuite",
                Language = new Language { Name = "English" },
                LanguageId = 200,
                PrototypeId = 300,
                Threshold = 50,
                QuizResponseTime = 10
            };

            // Act
            WordSuiteEditModel result = (new WordSuiteMapper()).MapForEdit(initial);

            // Assert
            Assert.AreEqual(initial.Id, result.Id);
            Assert.AreEqual(initial.Name, result.Name);
            Assert.AreEqual(initial.Language.Name, result.Language);
            Assert.AreEqual(initial.LanguageId, result.LanguageId);
            Assert.AreEqual(initial.PrototypeId, result.PrototypeId);
            Assert.AreEqual(initial.Threshold, result.Threshold);
            Assert.AreEqual(initial.QuizResponseTime, result.QuizResponseTime);
        }
    }           
}