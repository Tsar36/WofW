using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.Domain.Services.Services;
using WorldOfWords.Infrastructure.Data.EF.UnitOfWork;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using WorldOfWords.Domain.Models;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Globalization;

namespace WorldOfWords.Tests.ServicesTests
{
    [TestFixture]
    class LanguageServiceTest
    {
        Mock<IUnitOfWorkFactory> factory;
        Mock<IWorldOfWordsUow> uow;
        Mock<IRepository<Language>> repository;
        Mock<ILanguageMapper> mapper;
        Mock<IPartOfSpeechMapper> partsMapper;
        LanguageService service;

        [SetUp]
        public void SetUp()
        {
            factory = new Mock<IUnitOfWorkFactory>();
            uow = new Mock<IWorldOfWordsUow>();
            repository = new Mock<IRepository<Language>>();
            mapper = new Mock<ILanguageMapper>();
            partsMapper = new Mock<IPartOfSpeechMapper>();

            service = new LanguageService(factory.Object, mapper.Object, partsMapper.Object);

            factory.Setup(f => f.GetUnitOfWork()).Returns(uow.Object);
            uow.Setup(u => u.LanguageRepository).Returns(repository.Object);
        }
        #region oldTests
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
        public async Task AddLanguage_ReturnsNegative_IfLanguageExists()
        {
            //Arange
            string Name = "testName";
            int Id = 5;
            int expected = -1;

            LanguageModel languageModel = new LanguageModel { Id = Id };
            Language language = new Language { Name = Name, Id = Id };

            IQueryable<Language> languages = new List<Language>
            {
                new Language
                {
                    Id = Id,
                    Name=Name
                }
            }.AsQueryable<Language>();

            var langDb = GenerateMockDbSet<Language>(languages);
            repository.Setup(s => s.GetAll()).Returns(langDb.Object);
            mapper.Setup(m => m.ToDomainModel(It.IsAny<LanguageModel>())).Returns(language);
            

            //Act
            int actual = await service.AddAsync(languageModel);

            //Assert

            factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            uow.Verify(u => u.LanguageRepository, Times.Once);
            repository.Verify(w => w.GetAll(), Times.Once);
            Assert.AreEqual(actual, expected);
        }

        [Test]
        public async Task AddLanguage_ReturnsLanguageId_IfLanguageNotExists()
        {
            int expected = 1;
            LanguageModel language = new LanguageModel { Id = expected };
            Language language1 = new Language { Id = expected };
            mapper.Setup(m => m.ToDomainModel(It.IsAny<LanguageModel>())).Returns(language1);
            repository.Setup(r => r.Add(It.IsAny<Language>())).Verifiable();
            uow.Setup(u => u.SaveAsync()).ReturnsAsync(expected);

            //Act
            var actual = await service.AddAsync(language);

            //Assert
            factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            uow.Verify(u => u.LanguageRepository, Times.Exactly(2));
            repository.VerifyAll();
            mapper.Verify(m => m.ToDomainModel(It.IsAny<LanguageModel>()), Times.Once);
            uow.Verify(u => u.SaveAsync(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task RemoveLanguage_ReturnsNegative_LanguageNull()
        {
            int id = 1;
            Language lang = null;

            repository.Setup(l => l.GetById(id)).Returns(lang);

            //Act
            var actual = await service.RemoveAsync(id);

            //Assert
            repository.Verify(l => l.GetById(It.IsAny<int>()), Times.Once);
            factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            uow.Verify(u => u.LanguageRepository, Times.Once);
            Assert.IsFalse(actual);
        }

        [Test]
        public async Task RemoveLanguage_ReturnsNegative_NoCourseCount()
        {
            int id = 1;

            ICollection<Course> course = new List<Course>
            {
                new Course
                {
                    Id=4
                }
            };

            Language lang = new Language
            {
                Id = id,
                Courses = course
            };

            repository.Setup(l => l.GetById(id)).Returns(lang);

            //Act
            var actual = await service.RemoveAsync(id);

            //Assert
            repository.Verify(l => l.GetById(It.IsAny<int>()), Times.Once);
            factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            uow.Verify(u => u.LanguageRepository, Times.Once);
            Assert.IsFalse(actual);
        }

        [Test]
        public async Task RemoveLanguage_ReturnsNegative_NoWords_NoWordsuites()
        {
            int id = 1;

            LanguageModel lang = new LanguageModel
            {
                Id = id,
            };

            mapper.Setup(m => m.ToApiModel(It.IsAny<Language>())).Returns(lang);

            var actual = await service.RemoveAsync(id);

            //Assert
            repository.VerifyAll();
            uow.VerifyAll();

            repository.Verify(l => l.GetById(It.IsAny<int>()), Times.Once);
            factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            uow.Verify(u => u.LanguageRepository, Times.Once);
            Assert.IsFalse(actual);
        }

        [Test]
        public async Task RemoveLanguage_ReturnsNegative_WordsExist_WordsuitesExist()
        {
            //Arange
            Mock<IWordSuiteRepository> suiteRepository = new Mock<IWordSuiteRepository>();
            Mock<IWordProgressRepository> progressRepository = new Mock<IWordProgressRepository>();
            Mock<IWordTranslationRepository> translationRepository = new Mock<IWordTranslationRepository>();
            Mock<IRepository<Word>> wordRepository = new Mock<IRepository<Word>>();
            
            uow.Setup(u => u.WordSuiteRepository).Returns(suiteRepository.Object);
            uow.Setup(u => u.WordProgressRepository).Returns(progressRepository.Object);
            uow.Setup(u => u.WordTranslationRepository).Returns(translationRepository.Object);
            uow.Setup(u => u.WordRepository).Returns(wordRepository.Object);
            
            int id = 1;

            IQueryable<WordSuite> suite = new List<WordSuite>
            {
                new WordSuite
                {
                    LanguageId = id                     
                }
            }.AsQueryable<WordSuite>();

            var suiteDb = GenerateMockDbSet<WordSuite>(suite);
            suiteRepository.Setup(s => s.GetAll()).Returns(suiteDb.Object);
            //_suite.Setup(s => s.GetAll()).Returns(suite);

            IQueryable<WordProgress> prog = new List<WordProgress>
            {
                new WordProgress
                {
                   WordSuite = new WordSuite()
                }
            }.AsQueryable<WordProgress>();

            var progdb = GenerateMockDbSet<WordProgress>(prog);
            progressRepository.Setup(s => s.GetAll()).Returns(progdb.Object);
            //_prog.Setup(p => p.GetAll()).Returns(prog);

            IQueryable<WordTranslation> tran = new List<WordTranslation>
            {
                new WordTranslation
                {
                    OriginalWord=new Word
                    {
                        LanguageId=id                    
                    }
                }
            }.AsQueryable<WordTranslation>();

            var transDb = GenerateMockDbSet<WordTranslation>(tran);
            translationRepository.Setup(s => s.GetAll()).Returns(transDb.Object);
            //_trans.Setup(t => t.GetAll()).Returns(tran);

            IQueryable<Word> word = new List<Word>
            {
                new Word
                {
                    LanguageId = id
                }
            }.AsQueryable<Word>();

            var wordDb = GenerateMockDbSet<Word>(word);
            wordRepository.Setup(s => s.GetAll()).Returns(wordDb.Object);
            //_word.Setup(w => w.GetAll()).Returns(word);

            ICollection<WordSuite> wordSuitesCollection = new List<WordSuite> { new WordSuite { Id = 8 } };
            ICollection<Word> wordCollection = new List<Word> { new Word { Id = 5 } };

            Language lang = new Language
            {
                Id = id,
                Name = "LanguageName",
                WordSuites = wordSuitesCollection,
                Words = wordCollection
            };

            repository.Setup(l => l.GetById(id)).Returns(lang);
            progressRepository.Setup(p => p.Delete(It.IsAny<IEnumerable<WordProgress>>())).Verifiable();
            suiteRepository.Setup(s => s.Delete(It.IsAny<IEnumerable<WordSuite>>())).Verifiable();
            translationRepository.Setup(t => t.Delete(It.IsAny<IEnumerable<WordTranslation>>())).Verifiable();
            wordRepository.Setup(w => w.Delete(It.IsAny<IEnumerable<Word>>())).Verifiable();
            repository.Setup(l => l.Delete(It.IsAny<Language>())).Verifiable();
            uow.Setup(u => u.SaveAsync()).ReturnsAsync(It.IsAny<int>());

            //Act
            var actual = await service.RemoveAsync(id);

            //Assert
            repository.VerifyAll();
            uow.VerifyAll();
            wordRepository.VerifyAll();
            translationRepository.VerifyAll();
            suiteRepository.VerifyAll();
            progressRepository.VerifyAll();

            repository.Verify(l => l.GetById(It.IsAny<int>()), Times.Once);
            factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            uow.Verify(u => u.LanguageRepository, Times.Exactly(2));
            uow.Verify(u => u.WordProgressRepository, Times.Exactly(2));
            uow.Verify(u => u.WordSuiteRepository, Times.Exactly(2));
            uow.Verify(u => u.WordTranslationRepository, Times.Exactly(2));
            uow.Verify(u => u.WordRepository, Times.Exactly(2));
            wordRepository.Verify(w => w.GetAll(), Times.Once);
            translationRepository.Verify(w => w.GetAll(), Times.Once);
            suiteRepository.Verify(w => w.GetAll(), Times.Once);
            progressRepository.Verify(w => w.GetAll(), Times.Once);
            wordRepository.Verify(w => w.Delete(It.IsAny<IEnumerable<Word>>()), Times.Once);
            translationRepository.Verify(w => w.Delete(It.IsAny<IEnumerable<WordTranslation>>()), Times.Once);
            suiteRepository.Verify(w => w.Delete(It.IsAny<IEnumerable<WordSuite>>()), Times.Once);
            progressRepository.Verify(w => w.Delete(It.IsAny<IEnumerable<WordProgress>>()), Times.Once);
            repository.Verify(w => w.Delete(It.IsAny<Language>()), Times.Once);
            uow.Verify(u => u.SaveAsync(), Times.Once);

            Assert.IsTrue(actual);
        }


        //+
        [Test]
        public async Task GetAllLanguages_ReturnsListOfWords()
        {
            //Arange
            List<LanguageModel> expected = new List<LanguageModel>
            {
                new LanguageModel
                {
                    Id = 2
                }
            };

            IQueryable<Language> languages = new List<Language>
             {
                 new Language
                 {
                     Id=2
                     
                 }
             }.AsQueryable<Language>();

            mapper.Setup(m => m.ToApiModel(It.IsAny<Language>())).Returns(expected[0]);

            var languagesDB = GenerateMockDbSet<Language>(languages);
            repository.Setup(r => r.GetAll()).Returns(languagesDB.Object);

            //Act
            var actual = await service.GetLanguagesAsync();

            //Assert
            factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            uow.Verify(u => u.LanguageRepository, Times.Once);
            repository.Verify(l => l.GetAll(), Times.Once);

            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion
        // Written by Andriy Kusiy
        //
        [Test]
        public void GetWorldLanguages_ReturnsEnumerableOfLanguageModels()
        {
            //Arrange
            LanguageModel fake = new LanguageModel();
            mapper.Setup(m => m.CuntureInfoToLanguageModel(It.IsAny<CultureInfo>())).
                Returns(fake);
            var expected = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Select(c => mapper.Object.CuntureInfoToLanguageModel(c));
          
            //Act
            var actual = service.GetWorldLanguages();

            //Assert
            Assert.IsNotNull(actual);
            CollectionAssert.AreEqual(expected, actual);      
        }

        [Test]
        public async void GetLanguageByIDAsync_ReturnsApiModel_Positive()
        {
            //Arrange
            int id = 1;
            LanguageModel langModel = new LanguageModel
            {
                Id = id
            };

            Language lang = new Language
            {
                Id = id
            };

            repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(lang);
            mapper.Setup(m => m.ToApiModel(lang)).Returns(langModel);
              
            //Act
            var actual = await service.GetlanguageByIDAsync(id);

            //Assert
            repository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
            mapper.Verify(m => m.ToApiModel(It.IsAny<Language>()), Times.Once);
            Assert.AreEqual(actual, langModel);
        }

    }
}
