//Oleh Krutii
//Review: Nazar Krypiak
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WorldOfWords.Domain.Services;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.Infrastructure.Data.EF.UnitOfWork;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using WorldOfWords.Domain.Services.Services;
using WorldOfWords.API.Models;


namespace WorldOfWords.Tests.ServicesTests
{
    [TestFixture]
    class WordSeviceTest
    {
        private Mock<IUnitOfWorkFactory> _factory;
        private Mock<IWorldOfWordsUow> _uow;
        private Mock<IRepository<Word>> _words;
        private WordService _service;
        private Mock<IWordMapper> _mapper;

        [SetUp]
        public void Setup()
        {
            _factory = new Mock<IUnitOfWorkFactory>();
            _uow = new Mock<IWorldOfWordsUow>();
            _words = new Mock<IRepository<Word>>();
            _mapper = new Mock<IWordMapper>();
            _service = new WordService(_factory.Object, _mapper.Object);

            _factory.Setup(f => f.GetUnitOfWork()).Returns(_uow.Object);
            _uow.Setup(u => u.WordRepository).Returns(_words.Object);
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
        public async Task GetAllWords_ReturnsListOfWords()
        {
            //Arrange
            IQueryable<Word> expected = new List<Word>
                            {
                                new Word 
                                {
                                    Id = 8
                                }
                            }.AsQueryable<Word>();

            var mockSet = GenerateMockDbSet<Word>(expected);
            _words.Setup(w => w.GetAll()).Returns(mockSet.Object);

            //Act
            var actual = await _service.GetAllWordsAsync();

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordRepository, Times.Once);
            _words.Verify(w => w.GetAll(), Times.Once);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAllWords_ThrowsArgumentNullException()
        {
            //Arrange
            IQueryable<Word> que = null;
            _words.Setup(w => w.GetAll()).Returns(que);

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(async () => await _service.GetAllWordsAsync());
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordRepository, Times.Once);
            _words.Verify(w => w.GetAll(), Times.Once);
        }

        [Test]
        public async Task GetTopBySearchWord_ReturnsListOfWords()
        {
            //Arrange
            string searchWord = "Pes";
            int lanId = 88;
            int count = 5;
            IQueryable<Word> expected = new List<Word>
                            {
                                new Word 
                                {
                                    Value = "prostPes",
                                    LanguageId = lanId,
                                    Id = 8
                                }
                            }.AsQueryable<Word>();
            var mockSet = GenerateMockDbSet<Word>(expected);
            _words.Setup(w => w.GetAll()).Returns(mockSet.Object);

            //Act
            var actual = await _service.GetTopBySearchWordAsync(searchWord, lanId, count);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordRepository, Times.Once);
            _words.Verify(w => w.GetAll(), Times.Once);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GetTopBySearchWord_ThrowsArgumentNullException()
        {
            //Arrange
            string searchWord = "Pes";
            int lanId = 88;
            int count = 5;
            IQueryable<Word> nullik = null;
            _words.Setup(w => w.GetAll()).Returns(nullik);

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(async () => await _service.GetTopBySearchWordAsync(searchWord, lanId, count));
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordRepository, Times.Once);
            _words.Verify(w => w.GetAll(), Times.Once);
        }

        [Test]
        public async Task GetAllWordsBySpecificLanguage_ReturnsListOfWords()
        {
            //Arrange
            int languageId = 8;
            IQueryable<Word> expected = new List<Word>
                            {
                                new Word
                                {
                                    LanguageId = languageId
                                },
                                new Word
                                {
                                    LanguageId = languageId
                                }
                            }.AsQueryable<Word>();
            var mockSet = GenerateMockDbSet<Word>(expected);
            _words.Setup(w => w.GetAll()).Returns(mockSet.Object);

            //Act
            var actual = await _service.GetAllWordsBySpecificLanguageAsync(languageId);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordRepository, Times.Once);
            _words.Verify(w => w.GetAll(), Times.Once);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAllWordsBySpecificLanguage_ThrowsArgNullException()
        {
            //Arrange
            int languageId = 8;
            IQueryable<Word> nullik = null;
            _words.Setup(w => w.GetAll()).Returns(nullik);

            //Act
            
            //Assert
            Assert.Throws<ArgumentNullException>(async () => await _service.GetAllWordsBySpecificLanguageAsync(languageId));
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordRepository, Times.Once);
            _words.Verify(w => w.GetAll(), Times.Once);
        }

        [Test]
        public async Task Exists_ReturnsId_IfWordIdExists()
        {
            //Arrange
            string value = "value";
            int languageId = 8;
            int expected = 5;
            IQueryable<Word> words = new List<Word>
                             {
                                 new Word
                                 {
                                     Id = expected,
                                     Value = value,
                                     LanguageId = languageId
                                 }
                             }.AsQueryable<Word>();
            var mockSet = GenerateMockDbSet<Word>(words);
            _words.Setup(w => w.GetAll()).Returns(mockSet.Object);

            //Act
            int actual = await _service.ExistsAsync(value, languageId);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordRepository, Times.Once);
            _words.Verify(w => w.GetAll(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task Exists_ReturnsZero_IfNoWords()
        {
            //Arrange
            string value = "value";
            int languageId = 8;
            int expected = 0;
            IQueryable<Word> words = new List<Word>
                             {
                                 new Word
                                 {
                                     Id = expected,
                                     Value = value,
                                     LanguageId = 4
                                 }
                             }.AsQueryable<Word>();
            var mockSet = GenerateMockDbSet<Word>(words);
            _words.Setup(w => w.GetAll()).Returns(mockSet.Object);

            //Act
            int actual = await _service.ExistsAsync(value, languageId);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordRepository, Times.Once);
            _words.Verify(w => w.GetAll(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Exists_ThrowsArgNullException()
        {
            //Arrange
            string value = "value";
            int languageId = 8;
            //int expected = 5;
            IQueryable<Word> nullik = null;
            _words.Setup(w => w.GetAll()).Returns(nullik);

            //Act
            
            //Assert
            Assert.Throws<ArgumentNullException>(async () => await _service.ExistsAsync(value, languageId));
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordRepository, Times.Once);
            _words.Verify(w => w.GetAll(), Times.Once);
        }
        //!!!This tests fail because mappers where moved from controller into service

        [Test]
        public async Task Add_RuturnsNegativeOne_IfWordExists_WS()
        {
            //Arrange
            string value = "value";
            int Id = 8;
            int expected = -1;
            IQueryable<Word> words = new List<Word>
                             {
                                 new Word
                                 {
                                     Id = Id,
                                     Value = value,
                                     LanguageId = Id
                                 }
                             }.AsQueryable<Word>();
            WordModel word = new WordModel { Value = value, LanguageId = Id };
            var mockSet = GenerateMockDbSet<Word>(words);
            _words.Setup(w => w.GetAll()).Returns(mockSet.Object);

            //Act
            int actual = await _service.AddAsync(word);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Exactly(2));
            _uow.Verify(u => u.WordRepository, Times.Once);
            _words.Verify(w => w.GetAll(), Times.Once);
            Assert.AreEqual(actual, expected);
        }

    

        [Test]
        public void Add_ThrowsArgNullException_WS()
        {
            //Arrange
            string value = "value";
            int Id = 8;
            int expected = Id;
            IQueryable<Word> words = null;
            WordModel word = new WordModel
            {
                Id = Id,
                Value = value,
                LanguageId = Id
            };

            _words.Setup(w => w.GetAll()).Returns(words);

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(async () => await _service.AddAsync(word));
            _factory.Verify(f => f.GetUnitOfWork(), Times.Exactly(2));
            _uow.Verify(u => u.WordRepository, Times.Once);
            _words.Verify(w => w.GetAll(), Times.Once);
        }

        [Test]
        public async Task GetWordById_ReturnsWord()
        {
            //Arrange
            Word expected = new Word();
            _words.Setup(w => w.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(expected);

            //Act
            Word actual = await _service.GetWordByIdAsync(It.IsAny<int>());

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordRepository, Times.Once);
            _words.Verify(w => w.GetByIdAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(expected, actual);
        }

    }
}
