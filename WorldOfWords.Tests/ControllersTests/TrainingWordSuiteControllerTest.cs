using System.Web.Http;
using System.Web.Http.Results;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using WorldofWords.Controllers;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Models;
using Moq;
using WorldOfWords.Domain.Services;
using System.Threading.Tasks;

namespace WorldOfWords.Tests.ControllersTests
{
    //class TrainingWordSuiteControllerTest
    //{
    //    private void GenerateData(string name, string[] roles)
    //    {
    //        GenericIdentity identity = new GenericIdentity(name);
    //        Thread.CurrentPrincipal =
    //            new GenericPrincipal(
    //                identity,
    //                roles
    //                );
    //    }
    //    [Test]
    //    public void TrainingWordSuiteController_GetTest()
    //    {
    //        //Arrange
    //        var initial = new WordSuite()
    //        {
    //            Id = 1,
    //            Name = "Days of the week"
    //        };

    //        var expected = new TrainingWordSuiteModel()
    //        {
    //            Id = 1,
    //            Name = "Days of the week",
    //            WordTranslations = new List<WordTranslationModel>()
    //                    {
    //                        new WordTranslationModel
    //                        {
    //                            Id = 1, OriginalWord = "sunday"
    //                        },
    //                        new WordTranslationModel
    //                        {
    //                            Id = 1, OriginalWord = "monday"
    //                        }
    //                    }
    //        };
    //        //Action
    //        Mock<IQuizWordSuiteMapper> testWordSuiteMapper = new Mock<IQuizWordSuiteMapper>();
    //        Mock<ITrainingWordSuiteMapper> trainingWordSuiteMapper = new Mock<ITrainingWordSuiteMapper>();
    //        Mock<IWordSuiteService> wordSuiteService = new Mock<IWordSuiteService>();
    //        Mock<IWordProgressService> progressService = new Mock<IWordProgressService>();
    //        Mock<IWordProgressMapper> progressMapper = new Mock<IWordProgressMapper>();
    //        Mock<IWordTranslationService> wordTranslationService = new Mock<IWordTranslationService>();
    //        GenerateData("1", new[] { "NoRoles" });
    //        TrainingWordSuiteController Controller = new TrainingWordSuiteController(testWordSuiteMapper.Object,
    //            trainingWordSuiteMapper.Object,
    //            wordSuiteService.Object,
    //            progressService.Object,
    //            progressMapper.Object,
    //            wordTranslationService.Object);
    //        wordSuiteService.Setup(x => x.GetWithNotStudiedWordsAsync(1)).ReturnsAsync(initial);
    //        testWordSuiteMapper
    //               .Setup(x => x.Map(initial))
    //                .Returns(expected);
    //        var actual = Controller.GetTask(1);

    //        //Assert
    //        //Assert.AreEqual(expected, actual);
    //    }

    //    [Test]
    //    public void TrainingWordSuiteController_Check()
    //    {
    //        //Arrange
    //        var initial = new WordSuite()
    //        {
    //            Id = 1,
    //            Name = "Days of the week",
    //        };

    //        var data = new TrainingWordSuiteModel()
    //        {
    //            Id = 1,
    //            Name = "Days of the week",
    //            QuizStartTime = DateTime.Now,
    //            QuizResponseTime = 10,
    //            WordTranslations = new List<WordTranslationModel>()
    //            {
    //                        new WordTranslationModel
    //                        {
    //                            Id = 1, OriginalWord = "sunday", TranslationWord = "sunday"
    //                        },
    //                        new WordTranslationModel
    //                        {
    //                            Id = 1, OriginalWord = "monday", TranslationWord = "monday"
    //                        }
    //                    }
    //        };

    //        var expected = new TrainingWordSuiteModel()
    //        {
    //            Id = 1,
    //            Name = "Days of the week",
    //            QuizStartTime = DateTime.Now,
    //            QuizResponseTime = 10,
    //            WordTranslations = new List<WordTranslationModel>()
    //                    {
    //                        new WordTranslationModel
    //                        {
    //                            Id = 1, OriginalWord = "sunday", TranslationWord = "неділя"
    //                        },
    //                        new WordTranslationModel
    //                        {
    //                            Id = 1, OriginalWord = "monday", TranslationWord = "понеділок"
    //                        }
    //                    }
    //        };

    //        //Action
    //        Mock<IQuizWordSuiteMapper> quizWordSuiteMapper = new Mock<IQuizWordSuiteMapper>();
    //        Mock<IQuizWordSuiteMapper> testWordSuiteMapper = new Mock<IQuizWordSuiteMapper>();
    //        Mock<ITrainingWordSuiteMapper> trainingWordSuiteMapper = new Mock<ITrainingWordSuiteMapper>();
    //        Mock<IWordSuiteService> wordSuiteService = new Mock<IWordSuiteService>();
    //        Mock<IWordProgressService> progressService = new Mock<IWordProgressService>();
    //        Mock<IWordProgressMapper> progressMapper = new Mock<IWordProgressMapper>();
    //        Mock<IWordTranslationService> wordTranslationService = new Mock<IWordTranslationService>();
    //        GenerateData("1", new[] { "NoRoles" });
    //        TrainingWordSuiteController Controller = new TrainingWordSuiteController(quizWordSuiteMapper.Object, trainingWordSuiteMapper.Object,
    //            wordSuiteService.Object,
    //            progressService.Object,
    //            progressMapper.Object,
    //            wordTranslationService.Object,
    //            testWordSuiteMapper.Object
    //           );
    //      wordSuiteService.Setup(x => x.GetWithNotStudiedWordsAsync(1)).ReturnsAsync(initial);
    //        trainingWordSuiteMapper
    //               .Setup(x => x.Map(initial))
    //                .Returns(expected);
    //        progressService.Setup(x => x.IncrementProgress(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

    //        var actual = Controller.CheckTask(data);

    //        //Assert
    //        Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<TrainingWordSuiteModel>), actual);

    //    }

    
}
