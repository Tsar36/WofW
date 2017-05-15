using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.Infrastructure.Data.EF.UnitOfWork;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using WorldOfWords.Domain.Services;
using WorldOfWords.Domain.Models;
using WorldOfWords.API.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace WorldOfWords.Tests.ServicesTests
{
    // TODO : Fix This test 
    [TestFixture]
    public class WordTranslationServiceTest
    {
        private Mock<IUnitOfWorkFactory> _factory;
        private Mock<IWorldOfWordsUow> _uow;
        private Mock<IWordTranslationRepository> _repo;
        private WordTranslationService _service;
        private Mock<IWordTranslationMapper> mapper;
        private Mock<IWordMapper> wordMapper;
        private Mock<ITagMapper> tagMapper;

        
        [SetUp]
        public void Setup()
        {
            mapper = new Mock<IWordTranslationMapper>();
            wordMapper = new Mock<IWordMapper>();
            tagMapper = new Mock<ITagMapper>();
            _factory = new Mock<IUnitOfWorkFactory>();
            _uow = new Mock<IWorldOfWordsUow>();

            _service = new WordTranslationService(_factory.Object, mapper.Object, wordMapper.Object, tagMapper.Object);
            _repo = new Mock<IWordTranslationRepository>();

            _factory.Setup(f => f.GetUnitOfWork()).Returns(_uow.Object);
            _uow.Setup(u => u.WordTranslationRepository).Returns(_repo.Object);

        }

        public static Mock<IDbSet<T>> GenerateMockDbSet<T>(IQueryable<T> collection) where T : class
        {
            var mockSet = new Mock<IDbSet<T>>();
            mockSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(collection.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<WordSuite>(collection.Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(collection.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(collection.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(collection.GetEnumerator());

            return mockSet;
        }


        [Test]
        public void Exists_ReturnsId()
        {
            //Arrange
            int originalWordId = 1;
            int translationWordId = 2;
            int expected = 1;

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
            {
                new WordTranslation
                {
                    Id = 1,
                    OriginalWordId = originalWordId,
                    TranslationWordId = translationWordId
                }
            }.AsQueryable<WordTranslation>();
            _repo.Setup(x => x.GetAll()).Returns(wordTranslations);

            //Act
            var actual = _service.Exists(originalWordId, translationWordId);

            //Assert
            Assert.AreEqual(actual, expected);

            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordTranslationRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);
        }
        [Test]
        public void Exists_ReturnsZero()
        {
            //Arrange
            int originalWordId = 1;
            int translationWordId = 2;
            int expected = 0;

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
            {
                new WordTranslation
                {
                    OriginalWordId = 2,
                    TranslationWordId = translationWordId
                }
            }.AsQueryable<WordTranslation>();
            _repo.Setup(x => x.GetAll()).Returns(wordTranslations);

            //Act
            var actual = _service.Exists(originalWordId, translationWordId);

            //Assert
            Assert.AreEqual(actual, expected);

            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordTranslationRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);
        }
        [Test]
        public void Exists_GetAllReturnsNull()
        {
            //Arrange
            IQueryable<WordTranslation> wordTranslations = null;
            _repo.Setup(x => x.GetAll()).Returns(wordTranslations);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => _service.Exists(It.IsAny<int>(), It.IsAny<int>()));

            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordTranslationRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);
        }
        //TODO : FIX NEXT TESTS

//        [Test]
//        public async Task GetWordsFromIntervalAsync_ReturnsListOfWordTranslations()
//        {
//            //Arrange
//            int startOfInterval = 0;
//            int endOfInterval = 1;
//            int originLangId = 1;
//            int translLangId = 4;

//            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
//            {
//                new WordTranslation
//                {
//                    OriginalWordId = 1,
//                    OriginalWord = new Word
//                    {
//                        LanguageId = originLangId
//                    },
//                    TranslationWord = new Word
//                    {
//                        LanguageId = translLangId
//}
//                }
//            }.AsQueryable<WordTranslation>();
//            List<WordTranslationImportModel> expected = new List<WordTranslationImportModel>
//            {
//                new WordTranslationImportModel
//                {
//                    OriginalWordId = 1
//                }
//            };
//            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
//            _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);
//            mapper.Setup(m => m.MapToImportModel(wordTranslations.First())).Returns(expected[0]);

//            //Act
//            var actual = await _service.GetWordsFromIntervalAsync(startOfInterval, endOfInterval, originLangId, translLangId);

//            //Assert
//            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
//            _uow.Verify(x => x.WordTranslationRepository, Times.Exactly(3));
//            _repo.Verify(x => x.GetAll(), Times.Exactly(3));

//            CollectionAssert.AreEqual(expected, actual);

//        }
//        [Test]
//        public void GetWordsFromIntervalAsync_ThrowsException()
//        {
//            //Arrange
//            int startOfInterval = 1;
//            int startOfIntervalNegative = -1;
//            int startOfIntervalBad = 3;
//            int endOfInterval = 0;
//            int endOfIntervalBad = 3;
//            int originLangId = 1;
//            int translLangId = 4;

//            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
//                                    {
//                                        new WordTranslation
//                                        {
//                                            OriginalWordId = 1,
//                                            OriginalWord = new Word
//                                            {
//                                                LanguageId = originLangId
//                                            },
//                                            TranslationWord = new Word
//                                            {
//                                                LanguageId = translLangId
//                                            }
//                                        },
//                                        new WordTranslation
//                                        {
//                                            OriginalWordId = 2,
//                                            OriginalWord = new Word
//                                            {
//                                                LanguageId = originLangId
//                                            },
//                                            TranslationWord = new Word
//                                            {
//                                                LanguageId = translLangId
//}
//                                        }
//                                    }.AsQueryable<WordTranslation>();

//            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
//            _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);

//            //Act
//            //Assert
//            Assert.Throws<ArgumentException>(async () =>
//                await _service.GetWordsFromIntervalAsync(startOfInterval, endOfInterval, originLangId, translLangId), "Start of interval is bigger than end");
//            Assert.Throws<ArgumentException>(async () =>
//                await _service.GetWordsFromIntervalAsync(startOfIntervalBad, endOfInterval, originLangId, translLangId), "Start of interval is bigger than end");
//            Assert.Throws<ArgumentException>(async () =>
//                await _service.GetWordsFromIntervalAsync(startOfIntervalNegative, endOfInterval, originLangId, translLangId), "Start of interval is bigger than end");
//            Assert.Throws<ArgumentException>(async () =>
//                await _service.GetWordsFromIntervalAsync(startOfInterval, endOfIntervalBad, originLangId, translLangId), "Start of interval is bigger than end");

//            _factory.Verify(x => x.GetUnitOfWork(), Times.Exactly(4));
//            _uow.Verify(x => x.WordTranslationRepository, Times.Exactly(5));
//            _repo.Verify(x => x.GetAll(), Times.Exactly(5));
//        }
//        [Test]
//        public void GetWordsFromIntervalAsync_GetAllReturnsNull()
//        {
//            //Arrange
//            IQueryable<WordTranslation> wordTranslations = null;
//            _repo.Setup(x => x.GetAll()).Returns(wordTranslations);

//            //Act
//            //Assert
//            Assert.Throws<ArgumentNullException>(async () => await _service.GetWordsFromIntervalAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));

//            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
//            _uow.Verify(x => x.WordTranslationRepository, Times.Once);
//            _repo.Verify(x => x.GetAll(), Times.Once);
//        }

//        [Test]
//        public async Task GetWordsWithSearchValueAsync_ReturnsListOfWordTranslations()
//        {
//            //Arrange
//            int startOfInterval = 0;
//            int endOfInterval = 1;
//            int originLangId = 1;
//            int translLangId = 4;
//            string searchVal = "foo";

//            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
//            {
//                new WordTranslation
//                {
//                    OriginalWordId = 1,
//                    OriginalWord = new Word
//                    {
//                        Value = "fool",
//                        LanguageId = originLangId
//                    },
//                    TranslationWord = new Word
//                    {
//                        LanguageId = translLangId
//}
//                }
//            }.AsQueryable<WordTranslation>();
//            List<WordTranslationImportModel> expected = new List<WordTranslationImportModel>
//            {
//                new WordTranslationImportModel
//                {
//                    OriginalWordId = 1,
//                    OriginalWord = "fool"
//                }
//            };
//            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
//            _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);
//            mapper.Setup(m => m.MapToImportModel(wordTranslations.First())).Returns(expected[0]);

//            //Act
//            var actual = await _service.GetWordsWithSearchValueAsync(searchVal, startOfInterval, endOfInterval, originLangId, translLangId);

//            //Assert
//            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
//            _uow.Verify(x => x.WordTranslationRepository, Times.Exactly(3));
//            _repo.Verify(x => x.GetAll(), Times.Exactly(3));

//            CollectionAssert.AreEqual(expected, actual);

//        }
//        [Test]
//        public void GetWordsWithSearchValueAsync_ThrowsException()
//        {
//            //Arrange
//            int startOfInterval = 1;
//            int startOfIntervalNegative = -1;
//            int startOfIntervalBad = 3;
//            int endOfInterval = 0;
//            int endOfIntervalBad = 3;
//            int originLangId = 1;
//            int translLangId = 4;

//            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
//                                    {
//                                        new WordTranslation
//                                        {
//                                            OriginalWordId = 1,
//                                            OriginalWord = new Word
//                                            {
//                                                LanguageId = originLangId
//                                            },
//                                            TranslationWord = new Word
//                                            {
//                                                LanguageId = translLangId
//                                            }
//                                        },
//                                        new WordTranslation
//                                        {
//                                            OriginalWordId = 2,
//                                            OriginalWord = new Word
//                                            {
//                                                LanguageId = originLangId
//                                            },
//                                            TranslationWord = new Word
//                                            {
//                                                LanguageId = translLangId
//                                            }
//                                        }
//                                    }.AsQueryable<WordTranslation>();

//            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
//            _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);

//            //Act
//            //Assert
//            Assert.Throws<ArgumentException>(async () =>
//               await _service.GetWordsWithSearchValueAsync(It.IsAny<string>(), startOfInterval, endOfInterval, originLangId, translLangId), "Start of interval is bigger than end");
//            Assert.Throws<ArgumentException>(async () =>
//                await _service.GetWordsWithSearchValueAsync(It.IsAny<string>(), startOfIntervalBad, endOfInterval, originLangId, translLangId), "Start of interval is bigger than end");
//            Assert.Throws<ArgumentException>(async () =>
//                await _service.GetWordsWithSearchValueAsync(It.IsAny<string>(), startOfIntervalNegative, endOfInterval, originLangId, translLangId), "Start of interval is bigger than end");
//            Assert.Throws<ArgumentException>(async () =>
//                await _service.GetWordsWithSearchValueAsync(It.IsAny<string>(), startOfInterval, endOfIntervalBad, originLangId, translLangId), "Start of interval is bigger than end");

//            _factory.Verify(x => x.GetUnitOfWork(), Times.Exactly(4));
//            _uow.Verify(x => x.WordTranslationRepository, Times.Exactly(5));
//            _repo.Verify(x => x.GetAll(), Times.Exactly(5));

//        }
//        [Test]
//        public void GetWordsWithSearchValueAsync_GetAllReturnsNull()
//        {
//            //Arrange
//            IQueryable<WordTranslation> wordTranslations = null;
//            _repo.Setup(x => x.GetAll()).Returns(wordTranslations);

//            //Act
//            //Assert
//            Assert.Throws<ArgumentNullException>(async () =>
//                await _service.GetWordsWithSearchValueAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));

//            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
//            _uow.Verify(x => x.WordTranslationRepository, Times.Once);
//            _repo.Verify(x => x.GetAll(), Times.Once);
//        }

//        [Test]
//        public async Task GetAmountOfWordTranslationsByLanguageAsync_ReturnsAmount()
//        {
//            //Arrange
//            int originLangId = 1;
//            int translLangId = 4;
//            int actual = 1;

//            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
//                                    {
//                                        new WordTranslation
//                                        {
//                                            OriginalWordId = 1,
//                                            OriginalWord = new Word
//                                            {
//                                                LanguageId = originLangId
//                                            },
//                                            TranslationWord = new Word
//                                            {
//                                                LanguageId = translLangId
//                                            }
//                                        }
//                                    }.AsQueryable<WordTranslation>();

//            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
//            _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);

//            //Act
//            int expected = await _service.GetAmountOfWordTranslationsByLanguageAsync(originLangId, translLangId);

//            //Assert
//            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
//            _uow.Verify(x => x.WordTranslationRepository, Times.Once);
//            _repo.Verify(x => x.GetAll(), Times.Once);

//            Assert.AreEqual(actual, expected);

//        }
//        [Test]
//        public void GetAmountOfWordTranslationsByLanguageAsync_GetAllReturnsNull()
//        {
//            //Arrange
//            IQueryable<WordTranslation> wordTranslations = null;
//            _repo.Setup(x => x.GetAll()).Returns(wordTranslations);

//            //Act
//            //Assert
//            Assert.Throws<ArgumentNullException>(async () => await _service.GetAmountOfWordTranslationsByLanguageAsync(It.IsAny<int>(), It.IsAny<int>()));

//            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
//            _uow.Verify(x => x.WordTranslationRepository, Times.Once);
//            _repo.Verify(x => x.GetAll(), Times.Once);
//        }

        //[Test]
        //public async Task GetAmountOfWordsBySearchValuesAsync_ReturnsAmount()
        //{
        //    //Arrange
        //    int originLangId = 1;
        //    int translLangId = 4;
        //    string searchValue = "foo";
        //    int actual = 1;

        //    IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
        //                            {
        //                                new WordTranslation
        //                                {
        //                                    OriginalWordId = 1,
        //                                    OriginalWord = new Word
        //                                    {
        //                                        Value = searchValue,
        //                                        LanguageId = originLangId
        //                                    },
        //                                    TranslationWord = new Word
        //                                    {
        //                                        LanguageId = translLangId
        //                                    }
        //                                }
        //                            }.AsQueryable<WordTranslation>();

        //    var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
        //    _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);

        //    //Act
        //    int expected = await _service.GetAmountOfWordsBySearchValuesAsync(searchValue, originLangId, translLangId);

        //    //Assert
        //    _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
        //    _uow.Verify(x => x.WordTranslationRepository, Times.Once);
        //    _repo.Verify(x => x.GetAll(), Times.Once);

        //    Assert.AreEqual(actual, expected);

        //}
        // TODO : FIX
        //[Test]
        //public void GetAmountOfWordsBySearchValuesAsync_GetAllReturnsNull()
        //{
        //    //Arrange
        //    IQueryable<WordTranslation> wordTranslations = null;
        //    _repo.Setup(x => x.GetAll()).Returns(wordTranslations);

        //    //Act
        //    //Assert
        //    Assert.Throws<ArgumentNullException>(async () =>
        //        await _service.GetAmountOfWordsBySearchValuesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));

        //    _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
        //    _uow.Verify(x => x.WordTranslationRepository, Times.Once);
        //    _repo.Verify(x => x.GetAll(), Times.Once);
        //}

        //[Test]
        //public void GetWordFullInformationAsync_GetAllReturnsNull()
        //{
        //    //Arrange
        //    IQueryable<WordTranslation> wordTranslations = null;
        //    _repo.Setup(x => x.GetAll()).Returns(wordTranslations);

        //    //Act
        //    //Assert
        //    Assert.Throws<ArgumentNullException>(async () =>
        //        await _service.GetWordFullInformationAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));

        //    _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
        //    _uow.Verify(x => x.WordTranslationRepository, Times.Once);
        //    _repo.Verify(x => x.GetAll(), Times.Once);
        //}

        [Test]
        public async Task GetWordFullInformationStringsAsync_ReturnsWord()
        {
            //Arrange
            string word = "word";
            string description = "word";
            string transl = "слово";
            string synon = "phrase";
            int originLangId = 1;
            int translLangId = 4;

            Word translation = new Word()
            {
                Id = 2,
                Value = transl,
                LanguageId = translLangId
            };
            Word synonim = new Word()
            {
                Id = 3,
                Value = synon,
                LanguageId = originLangId
            };

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
            {
                new WordTranslation
                {
                    OriginalWordId = 1,
                    OriginalWord = new Word
                    {
                        Value = word,
                        Description = description,
                        LanguageId = originLangId
                    },
                    TranslationWord = translation
                },
                new WordTranslation
                {
                    OriginalWordId = 1,
                    OriginalWord = new Word
                    {
                        Value = word,
                        Description = description,
                        LanguageId = originLangId
                    },
                    TranslationWord = synonim
                }
            }.AsQueryable<WordTranslation>();

            WordTranslationFullStringsModel expected = new WordTranslationFullStringsModel
            {
                OriginalWord = word,
                Description = description,
                Translations = new List<string>
                {
                    transl
                },
                Synonims = new List<string>
                {
                    synon
                }
            };

            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
            _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);


            //Act
            var actual = await _service.GetWordFullInformationStringsAsync(word, originLangId, translLangId);

            //Assert
            Assert.AreEqual(expected.OriginalWord, actual.OriginalWord);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.Transcription, actual.Transcription);
            CollectionAssert.AreEqual(expected.Translations, actual.Translations);
            CollectionAssert.AreEqual(expected.Synonims, actual.Synonims);

            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordTranslationRepository, Times.Exactly(3));
            _repo.Verify(x => x.GetAll(), Times.Exactly(3));

        }
        [Test]
        public void GetWordFullInformationStringsAsync_GetAllReturnsNull()
        {
            //Arrange
            IQueryable<WordTranslation> wordTranslations = null;
            _repo.Setup(x => x.GetAll()).Returns(wordTranslations);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(async () =>
                await _service.GetWordFullInformationStringsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));

            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordTranslationRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);
        }
        
        [Test]
        public async Task DeleteAsync_IfDependencyDoesntExists()
        {
            //Arrange
            int originalId = 1;
            int translationId = 2;
            WordTranslation transl = new WordTranslation()
            {
                OriginalWordId = originalId,
                TranslationWordId = translationId
            };
            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
            {
                transl

            }.AsQueryable<WordTranslation>();

            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
            _repo.Setup(r => r.GetAll()).Returns(mockSet.Object);
            _uow.Setup(x => x.SaveAsync()).ReturnsAsync(1);
            _repo.Setup(r => r.Delete(originalId, translationId)).Verifiable();

            //Act
            var actual = await _service.DeleteAsync(originalId, translationId);

            //Assert
            Assert.IsTrue(actual);

            _factory.Verify(x => x.GetUnitOfWork(), Times.Exactly(2));
            _uow.Verify(x => x.WordTranslationRepository, Times.Exactly(2));
            _repo.Verify(x => x.GetAll(), Times.Once());
            _uow.Verify(x => x.SaveAsync(), Times.Once);
            _repo.VerifyAll();          

        }
        [Test]
        public async Task DeleteAsync_IfDependencyExists()
        {
            //Arrange
            int originalId = 1;
            int translationId = 2;
            ICollection<WordProgress> progresses = new List<WordProgress>
            {
                new WordProgress
                    {
                        WordTranslationId = originalId
                    }
            };
            WordTranslation transl = new WordTranslation()
            {
                OriginalWordId = originalId,
                TranslationWordId = translationId,
                WordProgresses = progresses
            };
            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
            {
                transl

            }.AsQueryable<WordTranslation>();

            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
            _repo.Setup(r => r.GetAll()).Returns(mockSet.Object);
            _uow.Setup(x => x.SaveAsync()).ReturnsAsync(1);

            //Act
            var actual = await _service.DeleteAsync(originalId, translationId);

            //Assert
            Assert.IsTrue(actual);

            _factory.Verify(x => x.GetUnitOfWork(), Times.Exactly(2));
            _uow.Verify(x => x.WordTranslationRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);
        }

        [Test]
        public async Task GetByIdWithWordsAsync_ReturnFirst_CorrectObject()
        {
            //Arrange
            int translationId = 1;
            _uow.Setup(u => u.WordTranslationRepository).Returns(_repo.Object);

            Word originalWord = new Word() { Id = 1 };
            Word translationWord = new Word() { Id = 1 };
            WordTranslation expected = new WordTranslation
            {
                Id = translationId,
                TranslationWord = translationWord,
                OriginalWord = originalWord
            };

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
            {
                new WordTranslation
                {
                    Id = translationId,
                    TranslationWord = translationWord,
                    OriginalWord = originalWord
                },
                new WordTranslation
                {
                    Id = 2,
                    TranslationWord = new Word { Id = 6 },
                    OriginalWord = new Word { Id = 8 }
                }
            }.AsQueryable<WordTranslation>();
            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);

            _repo.Setup(r => r.GetAll()).Returns(mockSet.Object);

            //Act
            var actual = await _service.GetByIdWithWordsAsync(translationId);
        
            //Assert
            _repo.Verify(r => r.GetAll(), Times.Once);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.OriginalWord.Id, actual.OriginalWord.Id);
            Assert.AreEqual(expected.TranslationWord.Id, actual.TranslationWord.Id);
        }

        [Test]
        public async Task GetByIdWithWordsAsync_WhereReturnNull()
        {
            //Arrange
            int translationId = 1;
            _uow.Setup(u => u.WordTranslationRepository).Returns(_repo.Object);
            WordTranslation someTranslation = new WordTranslation
            {
                Id = 14,
            };

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
            {
                someTranslation
            }.AsQueryable<WordTranslation>();
            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
            _repo.Setup(r => r.GetAll()).Returns(mockSet.Object);

            //Act
            var actual = await _service.GetByIdWithWordsAsync(translationId);

            //Assert
            _repo.Verify(r => r.GetAll(), Times.Once);
            Assert.IsNull(actual);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task GetWordFullInformationAsync_WordRepoGetAll_ThrowNullReference()
        {
            //Arrange
            string searchWordValue = "word";
            int searchWordsOriginalLanguageId = 1;
            int searchWordsLanguageId = 11;
            string fakeWordValue = "fake word";
            int fakeWordLanguageId = 1000;

            Word fakeWord = new Word
            {
                Value = fakeWordValue,
                LanguageId = fakeWordLanguageId,
            };

            IQueryable<Word> words = new List<Word>
            {
                fakeWord
            }.AsQueryable<Word>();
            var wordsMockSet = GenerateMockDbSet<Word>(words);

            var wordRepo = new Mock<IRepository<Word>>();
            wordRepo.Setup(wr => wr.GetAll()).Returns(wordsMockSet.Object);
            _uow.Setup(u => u.WordRepository).Returns(wordRepo.Object);

            //Act and Assert
            await _service.GetWordFullInformationAsync(searchWordValue, searchWordsOriginalLanguageId, searchWordsLanguageId);
        }

        [Test]
        public async Task GetWordFullInformationAsync_ReturnsWord()
        {
            //Arrange
            string word = "Word";
            string description = "word description";
            string translationWordValue = "слово";
            string synonWordValue = "phrase";
            string transcription = "transcription";
            int originalLangId = 101;
            int translationLangId = 110;

            Word originalWord = new Word
            {
                Value = word,
                Description = description,
                LanguageId = originalLangId
            };

            Word translationWord = new Word
            {
                Id = 222,
                Value = translationWordValue,
                LanguageId = translationLangId
            };
            Word synonymWord = new Word
            {
                Id = 111,
                Value = synonWordValue,
                LanguageId = originalLangId
            };

            Tag wordTag = new Tag
            {
                Name = "SuperTag"
            };

            IQueryable<Word> words = new List<Word>
            {
                new Word
                {
                    Value = word,
                    Description = description,
                    Transcription = transcription,
                    LanguageId = originalLangId,
                    Tags = new List<Tag> { wordTag }
                }
            }.AsQueryable<Word>();

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
            {
                new WordTranslation
                {
                    OriginalWordId = 1,
                    OriginalWord = originalWord,
                    TranslationWord = translationWord
                },
                new WordTranslation
                {
                    OriginalWordId = 1,
                    OriginalWord = originalWord,
                    TranslationWord = synonymWord
                }
            }.AsQueryable<WordTranslation>();

            WordTranslationFullModel expected = new WordTranslationFullModel
            {
                OriginalWord = word,
                Description = description,
                Translations = new List<WordValueModel>
                {
                    new WordValueModel
                    {
                        Value = translationWordValue
                    }
                },
                Synonims = new List<WordValueModel>
                {
                    new WordValueModel
                    {
                        Value = synonWordValue
                    }
                },
                Tags = new List<TagModel>
                {
                    new TagModel
                    {
                        Value = "SuperTag"
                    }
                }
            };

            var wordRepo = new Mock<IRepository<Word>>();
            var wordsMockSet = GenerateMockDbSet<Word>(words);
            wordRepo.Setup(wr => wr.GetAll()).Returns(wordsMockSet.Object);
            _uow.Setup(u => u.WordRepository).Returns(wordRepo.Object);
            
            var wtMockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
            _repo.Setup(r => r.GetAll()).Returns(wtMockSet.Object);

            wordMapper.Setup(wm => wm.ToValueModel(translationWord)).Returns(expected.Translations[0]);
            wordMapper.Setup(wm => wm.ToValueModel(synonymWord)).Returns(expected.Synonims[0]);
            tagMapper.Setup(tm => tm.Map(wordTag)).Returns(expected.Tags[0]);

            //Act
            var actual = await _service.GetWordFullInformationAsync(word, originalLangId, translationLangId);

            //Assert
            Assert.AreEqual(expected.OriginalWord, expected.OriginalWord);
            Assert.AreEqual(expected.Description, actual.Description);
            CollectionAssert.AreEqual(expected.Translations, actual.Translations);
            CollectionAssert.AreEqual(expected.Synonims, actual.Synonims);
            CollectionAssert.AreEqual(expected.Tags, actual.Tags);

            _factory.Verify(f => f.GetUnitOfWork(), Times.Exactly(3));
            _uow.Verify(u => u.WordRepository, Times.Once);
            _uow.Verify(u => u.WordTranslationRepository, Times.Exactly(4));
            _repo.Verify(r => r.GetAll(), Times.Exactly(4));
            wordRepo.Verify( wr => wr.GetAll(), Times.Once());

        }

        [Test]
        public async Task GetWordTranslationByIdAsync_ReturnWordTranslation()
        {
            //Arrange
            int searchId = 101;
            WordTranslation expected = new WordTranslation
            {
                Id = searchId
            };
            _repo.Setup(r => r.GetByIdAsync(searchId)).ReturnsAsync(expected);

            //Act
            var actual = await _service.GetWordTranslationByIdAsync(searchId);

            //Assert
            Assert.AreEqual(expected, actual);

            _uow.Verify(u => u.WordTranslationRepository, Times.Once);
            _repo.Verify(r => r.GetByIdAsync(searchId), Times.Once);
        }

        [Test]
        public async Task GetWordTranslationByIdAsync_ReturnNull()
        {
            //Arrange
            int searchId = 101;
            WordTranslation expected = null;

            _repo.Setup(r => r.GetByIdAsync(searchId)).ReturnsAsync(null);

            //Act
            var actual = await _service.GetWordTranslationByIdAsync(searchId);

            //Assert
            Assert.AreEqual(expected, actual);

            _uow.Verify(u => u.WordTranslationRepository, Times.Once);
            _repo.Verify(r => r.GetByIdAsync(searchId), Times.Once);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage="Start of interval is bigger than end")]
        public async Task GetWordsWithTagAsync_ThrowsArgumentException_FirstCondition()
        {
            //Arrange
            int startOfInteval = 1;
            int endOfInterval = 999;

            int someLangId = 101;
            int someTranslationLangId = 100;
            string someSearchValue = "search";

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>()
                .AsQueryable<WordTranslation>();
            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
            _repo.Setup(r => r.GetAll()).Returns(mockSet.Object);

            //Act and Assert
            await _service.GetWordsWithTagAsync(startOfInteval, endOfInterval, someLangId, someSearchValue, someTranslationLangId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Start of interval is bigger than end")]
        public async Task GetWordsWithTagAsync_ThrowsArgumentException_SecondCondition()
        {
            //Arrange
            int startOfInteval = -1;
            int endOfInterval = 999;

            int someLangId = 101;
            int someTranslationLangId = 100;
            string someSearchValue = "search";

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>()
                .AsQueryable<WordTranslation>();
            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
            _repo.Setup(r => r.GetAll()).Returns(mockSet.Object);

            //Act and Assert
            await _service.GetWordsWithTagAsync(startOfInteval, endOfInterval, someLangId, someSearchValue, someTranslationLangId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Start of interval is bigger than end")]
        public async Task GetWordsWithTagAsync_ThrowsArgumentException_ThirdCondition()
        {
            //Arrange
            int startOfInteval = 1;
            int endOfInterval = 0;

            int someLangId = 101;
            int someTranslationLangId = 100;
            string someSearchValue = "search";

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>() 
            {
                new WordTranslation(), new WordTranslation()
            }.AsQueryable<WordTranslation>();
            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
            _repo.Setup(r => r.GetAll()).Returns(mockSet.Object);

            //Act and Assert
            await _service.GetWordsWithTagAsync(startOfInteval, endOfInterval, someLangId, someSearchValue, someTranslationLangId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Start of interval is bigger than end")]
        public async Task GetWordsWithTagAsync_ThrowsArgumentException_FourthCondition()
        {
            //Arrange
            int startOfInteval = 0;
            int endOfInterval = 5;

            int someLangId = 101;
            int someTranslationLangId = 100;
            string someSearchValue = "search";

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>() 
            {
                new WordTranslation(), new WordTranslation()
            }.AsQueryable<WordTranslation>();
            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
            _repo.Setup(r => r.GetAll()).Returns(mockSet.Object);

            //Act and Assert
            await _service.GetWordsWithTagAsync(startOfInteval, endOfInterval, someLangId, someSearchValue, someTranslationLangId);
        }

        [Test]
        public async Task GetWordsWithTagAsync_ReturnsListOfImportModels()
        {
            //Arrange
            int startOfInteval = 0;
            int endOfInterval = 2;

            int originalLangId = 101;
            int translationLangId = 100;
            string searchValue = "search";

            WordTranslation wordTranslation1 = new WordTranslation
            {
                OriginalWord = new Word
                {
                    Id = 1,
                    Value = "april",
                    LanguageId = originalLangId,
                    Tags = new List<Tag> { new Tag { Name = searchValue } }
                },
                TranslationWord = new Word { LanguageId = translationLangId }
            };
            WordTranslation wordTranslation2 = new WordTranslation
            {
                OriginalWord = new Word
                {
                    Id = 2,
                    Value = "may",
                    LanguageId = originalLangId,
                    Tags = new List<Tag> { new Tag { Name = searchValue + "bla" } }
                },
                TranslationWord = new Word 
                { 
                    Id = 1,
                    Value = "травень",
                    LanguageId = translationLangId
                }
            };

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>() 
            {
                wordTranslation1, wordTranslation2
            }.AsQueryable<WordTranslation>();
            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
            _repo.Setup(r => r.GetAll()).Returns(mockSet.Object);

            WordTranslationImportModel model1 = new WordTranslationImportModel
            {
                OriginalWord = "april",
                OriginalWordId = 1,
                TranslationWord = "квітень",
                TranslationWordId = 1,
                LanguageId = originalLangId
            };
            WordTranslationImportModel model2 = new WordTranslationImportModel
            {
                OriginalWord = "may",
                OriginalWordId = 2,
                TranslationWord = "травень",
                TranslationWordId = 2,
                LanguageId = originalLangId
            };

            mapper.Setup(m => m.MapToImportModel(wordTranslation1)).Returns(model1);
            mapper.Setup(m => m.MapToImportModel(wordTranslation2)).Returns(model2);

            var expected = new List<WordTranslationImportModel>
            {
                model1, model2
            };

            //Act 
            var actual = await _service.GetWordsWithTagAsync(startOfInteval, endOfInterval, originalLangId, searchValue, translationLangId);

            //Assert
            CollectionAssert.AreEqual(expected, actual);
            _uow.Verify(u => u.WordTranslationRepository, Times.Exactly(3));
            _repo.Verify(r => r.GetAll(), Times.Exactly(3));
            mapper.Verify(m => m.MapToImportModel(wordTranslation1), Times.Once());
            mapper.Verify(m => m.MapToImportModel(wordTranslation2), Times.Once());
        }

        [Test]
        public async Task GetWordsWithTagAsync_ReturnsNull()
        {
            //Arrange
            int startOfInteval = 0;
            int endOfInterval = 1;

            int originalLangId = 101;
            int translationLangId = 100;
            string searchValue = "search";

            int fakeOriginalId = 111;
            int fakeTransLangId = 110;
            string fakeValue = "fake";

            WordTranslation wordTranslation1 = new WordTranslation
            {
                OriginalWord = new Word
                {
                    Id = 1,
                    Value = "april",
                    LanguageId = fakeOriginalId,
                    Tags = new List<Tag> { new Tag { Name = fakeValue } }
                },
                TranslationWord = new Word { LanguageId = fakeTransLangId }
            };

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>() 
            {
                wordTranslation1
            }.AsQueryable<WordTranslation>();
            var mockSet = GenerateMockDbSet<WordTranslation>(wordTranslations);
            _repo.Setup(r => r.GetAll()).Returns(mockSet.Object);

            var expected = new List<WordTranslationImportModel>();

            //Act 
            var actual = await _service.GetWordsWithTagAsync(startOfInteval, endOfInterval, originalLangId, searchValue, translationLangId);

            //Assert
            CollectionAssert.AreEqual(expected, actual);
            _uow.Verify(u => u.WordTranslationRepository, Times.Exactly(3));
            _repo.Verify(r => r.GetAll(), Times.Exactly(3));
        }

        [Test]
        public async Task AddAsyncTest()
        {
            //Arrange
            int expected = 1;
            WordTranslationImportModel wordTranslationModel = new WordTranslationImportModel();
            WordTranslation wordTranslation = new WordTranslation
            {
                Id = expected
            };
            _uow.Setup(_u => _u.SaveAsync()).ReturnsAsync(1);
            _repo.Setup(_r => _r.AddOrUpdate(It.IsAny<WordTranslation>()));
            mapper.Setup(m => m.Map(It.IsAny<WordTranslationImportModel>())).Returns(wordTranslation);
            
            //Act
            var actual = await _service.AddAsync(wordTranslationModel);
            
            //Assert
            _factory.Verify(_f => _f.GetUnitOfWork(), Times.Once());
            mapper.Verify(m => m.Map(It.IsAny<WordTranslationImportModel>()), Times.Once());
            _uow.Verify(u => u.WordTranslationRepository, Times.Once());
            _uow.Verify(_u => _u.SaveAsync(), Times.Once());
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task GetAllWordSynonymsById_ReturnsSynonyms()
        {
            //Arrange
            int Id_incoming = 1;
            var wordWithSynonyms = new Word
            {
                Id = Id_incoming,
                LanguageId = 1
            };
            IQueryable<WordTranslation> synonyms = new List<WordTranslation>
            {
                new WordTranslation
                {
                    OriginalWordId = 1,
                    OriginalWord = new Word
                    {
                        Id = 1,
                        LanguageId = 1
                    },
                    TranslationWordId = 2,
                    TranslationWord = new Word
                    {
                        Id = 2,
                        LanguageId = 1
                    }
                }
            }.AsQueryable<WordTranslation>();

            WordValueModel expected = new WordValueModel
            {
                Id = 2
            };

            var mockSet = GenerateMockDbSet<WordTranslation>(synonyms);
            _repo.Setup(_r => _r.GetAll()).Returns(mockSet.Object);
            wordMapper.Setup(_wm => _wm.ToValueModel(It.IsAny<Word>())).Returns(expected);

            //Act
            var actual = await _service.GetAllWordSynonymsById(Id_incoming);

            //Assert
            _factory.Verify(_f => _f.GetUnitOfWork(), Times.Once);
            wordMapper.Verify(_m => _m.ToValueModel(It.IsAny<Word>()), Times.Once);
            _repo.Verify(_re => _re.GetAll(), Times.Once);
            CollectionAssert.AreEqual(actual, new List<WordValueModel> { expected });
        }

        [Test]
        public async Task AddIdRangeAsync_notExists()
        {
            //Arrange
            int originalWordId = 1;
            int ownerId = 3;
            var wordTranslationIds = new List<int>
            {
                2
            };

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
            {
                new WordTranslation
                {
                    OriginalWordId = 4,
                    TranslationWordId = 5
                }
            }.AsQueryable<WordTranslation>();

            var mappedWord = new WordTranslation
            {
                Id = 2
            };

            _repo.Setup(_r => _r.GetAll()).Returns(wordTranslations);
            mapper.Setup(_m => _m.Map(It.IsAny<WordTranslationImportModel>())).Returns(mappedWord);
            _repo.Setup(_re => _re.AddOrUpdate(It.IsAny<WordTranslation>())).Verifiable();
            _uow.Setup(_u => _u.SaveAsync()).ReturnsAsync(1);
            
            //Act
            await _service.AddIdRangeAsync(originalWordId, wordTranslationIds, ownerId);

            //Assert
            _factory.Verify(_f => _f.GetUnitOfWork(), Times.Exactly(2));
            _repo.Verify(_r => _r.GetAll(), Times.Once());
            mapper.Verify(_m => _m.Map(It.IsAny<WordTranslationImportModel>()), Times.Once());
            _repo.Verify(_re => _re.AddOrUpdate(It.IsAny<WordTranslation>()), Times.Once);
            _uow.Verify(_a => _a.SaveAsync(), Times.Once());
        }

        [Test]
        public async Task AddIdRangeAsync_Exists()
        {
            //Arange
            int originalWordId = 1;
            int ownerId = 3;
            var wordTranslationIds = new List<int>
            {
                2
            };

            IQueryable<WordTranslation> wordTranslations = new List<WordTranslation>
            {
                new WordTranslation
                {
                    Id = 1,
                    OriginalWordId = 1,
                    TranslationWordId = 2
                }
            }.AsQueryable<WordTranslation>();

            _repo.Setup(_r => _r.GetAll()).Returns(wordTranslations);

            //Act
            await _service.AddIdRangeAsync(originalWordId, wordTranslationIds, ownerId);

            //Assert
            _factory.Verify(_f => _f.GetUnitOfWork(), Times.Exactly(1));
            _repo.Verify(_r => _r.GetAll(), Times.Once());
            mapper.Verify(_m => _m.Map(It.IsAny<WordTranslationImportModel>()), Times.Never());
            _repo.Verify(_re => _re.AddOrUpdate(It.IsAny<WordTranslation>()), Times.Never());
            _uow.Verify(_a => _a.SaveAsync(), Times.Never());
        }

        [Test]
        public async Task GetAmountOfTagsBySearchValuesAsync_ReturnsAmount()
        {
            //Arrange
            string searchValue = "Cartoon";
            int originalLangId = 1;
            int translationLangId = 2;

            IQueryable<WordTranslation> translations = new List<WordTranslation>
            {
                new WordTranslation
                {
                    OriginalWord = new Word
                    {
                        Id = 1,
                        LanguageId = 1,
                        Tags = new List<Tag>
                        {
                            new Tag
                            {
                                Id = 1,
                                Name = "Cartoon"
                            }
                        }
                    },
                    TranslationWord = new Word
                    {
                        Id = 2,
                        LanguageId = 2
                    }
                }
            }.AsQueryable<WordTranslation>();

            var DbMock = GenerateMockDbSet<WordTranslation>(translations);
            _repo.Setup(_r => _r.GetAll()).Returns(DbMock.Object);
            //Act
            var actual = await _service.GetAmountOfTagsBySearchValuesAsync(searchValue, originalLangId, translationLangId);

            //Assert
            _factory.Verify(_f => _f.GetUnitOfWork(), Times.Once());
            _repo.Verify(_r => _r.GetAll(), Times.Once());
            Assert.AreEqual(actual, 1);
        }

        [Test]
        public async Task GetAmountOfTranslations_ReturnsAmount()
        {
            //Arange
            int wordId = 1;
            int translLangId = 2;

            IQueryable<WordTranslation> translations = new List<WordTranslation>
            {
                new WordTranslation
                {
                    OriginalWord = new Word
                    {
                        Id = 1,
                        LanguageId = 1,
                    },
                    TranslationWord = new Word
                    {
                        Id = 2,
                        LanguageId = 2
                    }
                }
            }.AsQueryable<WordTranslation>();

            var dbMock = GenerateMockDbSet<WordTranslation>(translations);
            _repo.Setup(_r => _r.GetAll()).Returns(dbMock.Object);
            //Act
            var actual = await _service.GetAmountOfTranslations(wordId, translLangId);

            //Assert
            _factory.Verify(_f => _f.GetUnitOfWork(), Times.Once());
            _repo.Verify(_r => _r.GetAll(), Times.Once());
            Assert.AreEqual(actual, 1);
        }

        [Test]
        public async Task GetByIdAsync()
        {
            //Arrange
            int wordId = 1;
            WordTranslation expected = new WordTranslation
            {
                Id = wordId
            };
            
            _repo.Setup(_r => _r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync<IWordTranslationRepository, WordTranslation>(expected);

            //Act
            var actual = await _service.GetByIdAsync(wordId);

            //Assert
            _repo.Verify(_r => _r.GetByIdAsync(It.IsAny<int>()), Times.Once());
            _factory.Verify(_f => _f.GetUnitOfWork(), Times.Once());
            Assert.AreEqual(actual, expected);
        }
    }
}
