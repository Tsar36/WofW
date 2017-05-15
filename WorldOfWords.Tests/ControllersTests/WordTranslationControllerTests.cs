//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Security.Principal;
//using NUnit.Framework;
//using Moq;
//using WorldofWords.Controllers;
//using WorldOfWords.API.Models;
//using WorldOfWords.Domain.Models;
//using WorldOfWords.Domain.Services;

//namespace WorldOfWords.Tests.ControllersTests
//{
//    [TestFixture]
//    public class WordTranslationControllerTests
//    {
//        private void GenerateData(string name, string[] roles)
//        {
//            GenericIdentity identity = new GenericIdentity(name);
//            Thread.CurrentPrincipal = new GenericPrincipal(identity, roles);
//        }

//        [Test]
//        public void Get_WordTranslationsByWordSuiteID_idIsPositive_ReturnWordTranslationsList()
//        {            
//            Mock<IWordTranslationMapper> wordTranslationMapper = new Mock<IWordTranslationMapper>();
//            Mock<IWordTranslationService> wordTranslationService = new Mock<IWordTranslationService>();
//            Mock<IWordMapper> wordMapper = new Mock<IWordMapper>();
//            Mock<IWordService> wordService = new Mock<IWordService>();
//            GenerateData("1", new[] { "NoRoles" });

//            var id = 1;

//            var initial = new List<WordTranslation>()
//            {
//                new WordTranslation()
//                {
//                    Id = 1,
//                    OriginalWord = new Word()
//                    {
//                        Value = "Sunday"
//                    },
//                    TranslationWord = new Word()
//                    {
//                        Value = "неділя"
//                    }
//                },
//                new WordTranslation()
//                {
//                    Id = 2,
//                    OriginalWord = new Word()
//                    {
//                        Value = "Monday"
//                    },
//                    TranslationWord = new Word()
//                    {
//                        Value = "понеділок"
//                    }
//                }
//            };

//            var expected = new List<WordTranslationModel>()
//            {
//                new WordTranslationModel()
//                {
//                    Id = 1,
//                    OriginalWord = "Sunday",
//                    TranslationWord = "неділя"
//                },
//                new WordTranslationModel()
//                {
//                    Id = 2,
//                    OriginalWord = "Monday",
//                    TranslationWord = "понеділок"
//                }
//            };

//            WordTranslationController wordTranslationController = new WordTranslationController(
//                wordTranslationService.Object, 
//                wordTranslationMapper.Object, 
//                wordService.Object, 
//                wordMapper.Object);

//            wordTranslationService.Setup(x => x.GetByWordSuiteID(It.Is<int>(i => i > 0))).Returns(initial);

//            wordTranslationMapper.Setup(x => x.MapRange(initial)).Returns(expected);

//            var actual = wordTranslationController.Get(id);

//            Assert.AreEqual(expected, actual);
//        }
//        [Test]
//        public void Get_WordTranslationsByWordSuiteID_idIsNegativeOr0_ReturnArgumentException()
//        {
//            Mock<IWordTranslationMapper> wordTranslationMapper = new Mock<IWordTranslationMapper>();
//            Mock<IWordTranslationService> wordTranslationService = new Mock<IWordTranslationService>();
//            Mock<IWordMapper> wordMapper = new Mock<IWordMapper>();
//            Mock<IWordService> wordService = new Mock<IWordService>();
//            GenerateData("1", new[] { "NoRoles" });

//            WordTranslationController wordTranslationController = new WordTranslationController(
//                wordTranslationService.Object,
//                wordTranslationMapper.Object,
//                wordService.Object,
//                wordMapper.Object);

//            Assert.Throws<ArgumentException>(delegate() { wordTranslationController.Get(It.Is<int>(x => x <= 0)); });
//        }

//        [Test]
//        public void Get_WordTranslationsBySearchWord_searchWordIsNotEmpty_languageIdIsPositive_ReturnWordTranslationsList()
//        {
//            Mock<IWordTranslationMapper> wordTranslationMapper = new Mock<IWordTranslationMapper>();
//            Mock<IWordTranslationService> wordTranslationService = new Mock<IWordTranslationService>();
//            Mock<IWordMapper> wordMapper = new Mock<IWordMapper>();
//            Mock<IWordService> wordService = new Mock<IWordService>();
//            GenerateData("1", new[] { "NoRoles" });

//            var searchWord = "word";
//            var languageId = 1;

//            var initial = new List<WordTranslation>()
//            {
//                new WordTranslation()
//                {
//                    Id = 1,
//                    OriginalWord = new Word()
//                    {
//                        Value = "Someword"
//                    },
//                    TranslationWord = new Word()
//                    {
//                        Value = "слово"
//                    }
//                },
//                new WordTranslation()
//                {
//                    Id = 2,
//                    OriginalWord = new Word()
//                    {
//                        Value = "Wordsome"
//                    },
//                    TranslationWord = new Word()
//                    {
//                        Value = "слово"
//                    }
//                }
//            };

//            var expected = new List<WordTranslationModel>()
//            {
//                new WordTranslationModel()
//                {
//                    Id = 1,
//                    OriginalWord = "Someword",
//                    TranslationWord = "слово"
//                },
//                new WordTranslationModel()
//                {
//                    Id = 2,
//                    OriginalWord = "Wordsome",
//                    TranslationWord = "слово"
//                }
//            };

//            WordTranslationController wordTranslationController = new WordTranslationController(
//                wordTranslationService.Object,
//                wordTranslationMapper.Object,
//                wordService.Object,
//                wordMapper.Object);

//            wordTranslationService
//                .Setup(x => x
//                    .GetTopBySearchWord(It.Is<string>(s => s.Length > 0),
//                                        It.Is<int>(i => i > 0),
//                                        It.Is<int>(i => i > 0))).Returns(initial);

//            wordTranslationMapper.Setup(x => x.MapRange(initial)).Returns(expected);

//            var actual = wordTranslationController.Get(searchWord, languageId);

//            Assert.AreEqual(expected, actual);
//        }
//        [Test]
//        public void Get_WordTranslationsBySearchWord_searchWordIsNull_ReturnArgumentNullException()
//        {
//            Mock<IWordTranslationMapper> wordTranslationMapper = new Mock<IWordTranslationMapper>();
//            Mock<IWordTranslationService> wordTranslationService = new Mock<IWordTranslationService>();
//            Mock<IWordMapper> wordMapper = new Mock<IWordMapper>();
//            Mock<IWordService> wordService = new Mock<IWordService>();
//            GenerateData("1", new[] { "NoRoles" });

//            string searchWord = null;
//            var languageId = 1;

//            WordTranslationController wordTranslationController = new WordTranslationController(
//                wordTranslationService.Object,
//                wordTranslationMapper.Object,
//                wordService.Object,
//                wordMapper.Object);

//            Assert.Throws<ArgumentNullException>(delegate() { wordTranslationController.Get(searchWord, languageId); });
//        }
//        [Test]
//        public void Get_WordTranslationsBySearchWord_searchWordIsEmpty_ReturnArgumentException()
//        {
//            Mock<IWordTranslationMapper> wordTranslationMapper = new Mock<IWordTranslationMapper>();
//            Mock<IWordTranslationService> wordTranslationService = new Mock<IWordTranslationService>();
//            Mock<IWordMapper> wordMapper = new Mock<IWordMapper>();
//            Mock<IWordService> wordService = new Mock<IWordService>();
//            GenerateData("1", new[] { "NoRoles" });

//            var searchWord = String.Empty;
//            var languageId = 1;

//            WordTranslationController wordTranslationController = new WordTranslationController(
//                wordTranslationService.Object,
//                wordTranslationMapper.Object,
//                wordService.Object,
//                wordMapper.Object);

//            Assert.Throws<ArgumentException>(delegate() { wordTranslationController.Get(searchWord, languageId); });
//        }
//        [Test]
//        public void Get_WordTranslationsBySearchWord_languageIdIsNegativeOr0_ReturnArgumentException()
//        {
//            Mock<IWordTranslationMapper> wordTranslationMapper = new Mock<IWordTranslationMapper>();
//            Mock<IWordTranslationService> wordTranslationService = new Mock<IWordTranslationService>();
//            Mock<IWordMapper> wordMapper = new Mock<IWordMapper>();
//            Mock<IWordService> wordService = new Mock<IWordService>();
//            GenerateData("1", new[] { "NoRoles" });

//            var searchWord = "word";

//            WordTranslationController wordTranslationController = new WordTranslationController(
//                wordTranslationService.Object,
//                wordTranslationMapper.Object,
//                wordService.Object,
//                wordMapper.Object);

//            Assert.Throws<ArgumentException>(delegate() { wordTranslationController.Get(searchWord, It.Is<int>(i => i <= 0)); });
//        }
//    }
//}
