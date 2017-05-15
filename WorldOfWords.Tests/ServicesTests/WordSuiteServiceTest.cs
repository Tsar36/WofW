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
using WorldOfWords.API.Models;


namespace WorldOfWords.Tests.ServicesTests
{
    [TestFixture]
    class WordSuiteServiceTest
    {
        private Mock<IUnitOfWorkFactory> _factory;
        private Mock<IWorldOfWordsUow> _uow;
        private Mock<IWordSuiteRepository> _repo;
        private WordSuiteService _service;
        private Mock<IQuizMapper> _quizMapper;

        [SetUp]
        public void Setup()
        {
            _factory = new Mock<IUnitOfWorkFactory>();
            _uow = new Mock<IWorldOfWordsUow>();
            _quizMapper = new Mock<IQuizMapper>();
            _service = new WordSuiteService(_factory.Object, _quizMapper.Object);
            _repo = new Mock<IWordSuiteRepository>();

            _factory.Setup(x => x.GetUnitOfWork()).Returns(_uow.Object);
            _uow.Setup(x => x.WordSuiteRepository).Returns(_repo.Object);
        }
        #region oldtests
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
        public async Task GetByID_ReturnsWordSuite()
        {
            //Arrange
            int wordSuiteId = 1;
            WordSuite expected = new WordSuite
            {
                Id = wordSuiteId
            };
            IQueryable<WordSuite> suites = new List<WordSuite>()
                                    {
                                        expected
                                    }.AsQueryable<WordSuite>();

            var mockSet = GenerateMockDbSet<WordSuite>(suites);
            _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);

            //Act
            var actual = await _service.GetByIDAsync(wordSuiteId);

            //Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);

            Assert.AreEqual(expected, actual);

        }
        [Test]
        public void GetByID_ReturnsNull()
        {
            //Arrange
            int wordSuiteId = 1;
            IQueryable<WordSuite> suites = null;

            _repo.Setup(x => x.GetAll()).Returns(suites);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(async () => await _service.GetByIDAsync(wordSuiteId));

            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);


        }

        [Test]
        public async Task SetTimeTest()
        {
            //Arrange
            int id = 1;
            WordSuite wordSuite = new WordSuite
            {
                Id = id
            };

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(wordSuite);
            _uow.Setup(u => u.SaveAsync()).ReturnsAsync(It.IsAny<int>()).Verifiable();

            //Act
            await _service.SetTimeAsync(id);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _uow.VerifyAll();

        }
        [Test]
        public void SetTime_GetByIdNull()
        {
            //Arrange
            int id = 1;
            WordSuite wordSuite = null;

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(wordSuite);

            //Act
            //Assert
            Assert.Throws<NullReferenceException>(async () => await _service.SetTimeAsync(id));

            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);

        }

        [Test]
        public void RemoveWordSuitesForEnrollment_Positive()
        {
            //Arrange
            int enrollmentId = 1;
            int userId = 1;
            List<WordSuite> wordSuitesCollection = new List<WordSuite>() 
            {
                new WordSuite()
                {
                    Id = 1, 
                    PrototypeId = null
                },
                new WordSuite()
                {
                    Id = 1, 
                    OwnerId = userId,
                    PrototypeId = 1,
                    
                }
            };
            Enrollment enrollment = new Enrollment()
            {
                Id = enrollmentId,
                Group = new Group()
                {
                    Course = new Course()
                    {
                        WordSuites = wordSuitesCollection
                    }
                },
                User = new User() { Id = userId }
            };
            Mock<IRepository<Enrollment>> _enrollRepo = new Mock<IRepository<Enrollment>>();

            _uow.Setup(x => x.EnrollmentRepository).Returns(_enrollRepo.Object);
            _enrollRepo.Setup(x => x.GetById(It.IsAny<int>())).Returns(enrollment);
            _repo.Setup(x => x.Delete(It.IsAny<IEnumerable<WordSuite>>())).Verifiable();
            _uow.Setup(x => x.Save()).Verifiable();

            //Act
            _service.RemoveWordSuitesForEnrollment(enrollmentId);

            //Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.EnrollmentRepository, Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _enrollRepo.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _repo.Verify(x => x.Delete(It.IsAny<IEnumerable<WordSuite>>()), Times.Once);
            _uow.Verify(x => x.Save(), Times.Once);
            _repo.VerifyAll();
            _uow.VerifyAll();

        }
        [Test]
        public void RemoveWordSuitesForEnrollment_Negative()
        {
            //Arrange
            Enrollment enrollment = null;
            Mock<IRepository<Enrollment>> _enrollRepo = new Mock<IRepository<Enrollment>>();

            _uow.Setup(x => x.EnrollmentRepository).Returns(_enrollRepo.Object);
            _enrollRepo.Setup(x => x.GetById(It.IsAny<int>())).Returns(enrollment);

            //Act
            //Assert
            Assert.Throws<ArgumentException>(() => _service.RemoveWordSuitesForEnrollment(It.IsAny<int>()),
                                        "Enrollment with id you are requesting does not exist");

            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.EnrollmentRepository, Times.Once);
            _enrollRepo.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

        }

        [Test]
        public async Task GetWithNotStudiedWords_ReturnsWordSuite()
        {
            //Arrange
            int id = 1;
            ICollection<WordProgress> progressCollection = new List<WordProgress>()
            {
                new WordProgress() {Progress = 2},
                new WordProgress() {Progress = 6}
            };
            WordSuite expected = new WordSuite
            {
                Id = id,
                Threshold = 5,
                WordProgresses = progressCollection

            };
            IQueryable<WordSuite> wordSuites = new List<WordSuite>
                                    {
                                        expected
                                    }.AsQueryable<WordSuite>();

            var mockSet = GenerateMockDbSet<WordSuite>(wordSuites);
            _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);

            //Act
            var actual = await _service.GetWithNotStudiedWordsAsync(id);

            //Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);

            Assert.AreEqual(expected, actual);

        }
        [Test]
        public async Task GetWithNotStudiedWords_ReturnsNull()
        {
            //Arrange
            int id = 1;
            IQueryable<WordSuite> wordSuites = new List<WordSuite>
                                    {
                                        new WordSuite
                                        {
                                            Id = 2
                                        }
                                    }.AsQueryable<WordSuite>();

            var mockSet = GenerateMockDbSet<WordSuite>(wordSuites);
            _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);

            //Act
            var actual = await _service.GetWithNotStudiedWordsAsync(id);

            //Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);

            Assert.IsNull(actual);
        }
        [Test]
        public void GetWithNotStudiedWords_GetAllReturnsNull()
        {
            //Arrange
            int id = 1;
            IQueryable<WordSuite> wordSuites = null;
            _repo.Setup(x => x.GetAll()).Returns(wordSuites);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(async () => await _service.GetWithNotStudiedWordsAsync(id));

            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);

        }

        [Test]
        public void GetWordSuiteProgress_ReturnsProgress()
        {
            //Arrange
            int id = 1;
            var expected = 1000.0;
            ICollection<WordProgress> progressCollection = new List<WordProgress>()
            {
                new WordProgress() {Progress = 10},
                new WordProgress() {Progress = 10},
                new WordProgress() {Progress = 10}
            };
            WordSuite wordSuite = new WordSuite
            {
                Id = id,
                Threshold = 1,
                WordProgresses = progressCollection

            };
            IQueryable<WordSuite> wordSuites = new List<WordSuite>
                                    {
                                        wordSuite
                                    }.AsQueryable<WordSuite>();

            _repo.Setup(x => x.GetAll()).Returns(wordSuites);


            //Act
            var actual = _service.GetWordSuiteProgress(id);

            //Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);

            Assert.AreEqual(actual, expected);

        }
        [Test]
        public void GetWordSuiteProgress_ReturnsInfinity()
        {
            //Arrange
            int id = 1;
            ICollection<WordProgress> progressCollection = new List<WordProgress>()
            {
                new WordProgress() {Progress = 10},
                new WordProgress() {Progress = 10},
                new WordProgress() {Progress = 10}
            };
            WordSuite wordSuite = new WordSuite
            {
                Id = id,
                WordProgresses = progressCollection

            };
            IQueryable<WordSuite> wordSuites = new List<WordSuite>
                                    {
                                        wordSuite
                                    }.AsQueryable<WordSuite>();

            _repo.Setup(x => x.GetAll()).Returns(wordSuites);


            //Act
            var actual = _service.GetWordSuiteProgress(id);

            //Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);

            Assert.IsTrue(Double.IsInfinity(actual));

        }
        [Test]
        public void GetWordSuiteProgress_ReturnsZero()
        {
            int id = 1;
            var expected = 0.0;
            IQueryable<WordSuite> wordSuites = new List<WordSuite>
                                    {
                                        new WordSuite
                                        {
                                            Id = 2
                                        }
                                    }.AsQueryable<WordSuite>();

            _repo.Setup(x => x.GetAll()).Returns(wordSuites);

            //Act
            var actual = _service.GetWordSuiteProgress(id);

            //Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);

            Assert.AreEqual(actual, expected);
        }
        [Test]
        public void GetWordSuiteProgress_GetAllReturnsNull()
        {
            //Arrange
            int id = 1;
            IQueryable<WordSuite> wordSuites = null;

            _repo.Setup(x => x.GetAll()).Returns(wordSuites);


            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => _service.GetWordSuiteProgress(id));

            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);
        }

        [Test]
        public async void CopyWordsuitesForUsersByGroup_CopiesWordSuites()
        {
            //Arrange
            int groupId = 1;
            List<User> users = new List<User>()
            {
                new User() { Id = 1 },
                new User() { Id = 2 }
            };
            ICollection<WordSuite> wordSuites = new List<WordSuite>()
            {
                new WordSuite
                    {
                        PrototypeId = null
                    },
                new WordSuite
                    {
                        PrototypeId = 1
                    },
                new WordSuite
                    {
                        PrototypeId = null
                    }                
            };
            Group group = new Group()
            {
                Course = new Course()
                {
                    WordSuites = wordSuites
                }
            };
            Mock<IRepository<Group>> _groupRepo = new Mock<IRepository<Group>>();

            _uow.Setup(x => x.GroupRepository).Returns(_groupRepo.Object);
            _groupRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(group);
            _repo.Setup(x => x.Add(It.IsAny<List<WordSuite>>())).Verifiable();
            _uow.Setup(x => x.SaveAsync()).ReturnsAsync(It.IsAny<int>()).Verifiable();

            //Act
            await _service.CopyWordsuitesForUsersByGroupAsync(users, groupId);

            //Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.GroupRepository, Times.Once);
            _groupRepo.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.VerifyAll();
            _uow.VerifyAll();

        }
        [Test]
        public void CopyWordsuitesForUsersByGroup_ThrowsException()
        {
            //Arrange
            int id = 1;
            List<User> users = new List<User>();
            Group group = null;
            Mock<IRepository<Group>> _groupRepo = new Mock<IRepository<Group>>();

            _uow.Setup(x => x.GroupRepository).Returns(_groupRepo.Object);
            _groupRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(group);

            //Act
            //Assert            
            Assert.Throws<ArgumentException>(async () => await _service.CopyWordsuitesForUsersByGroupAsync(users, id),
                        "Group with id you are requesting does not exist");

            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.GroupRepository, Times.Once);
            _groupRepo.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void GetTeacherWordSuites_ReturnsListOfWordSuites()
        {
            //Arrange
            int ownerId = 1;
            IQueryable<WordSuite> expected = new List<WordSuite>
                                    {
                                        new WordSuite
                                        {
                                            PrototypeId = null,
                                            OwnerId = ownerId
                                        }
                                    }.AsQueryable<WordSuite>();

            var mockSet = GenerateMockDbSet<WordSuite>(expected);
            _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);

            //Act
            var task = _service.GetTeacherWordSuitesAsync(ownerId);
            task.Wait();

            //Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);

            CollectionAssert.AreEqual(expected.ToList<WordSuite>(), task.Result);
        }
        [Test]
        public async Task GetTeacherWordSuites_ReturnsEmpty()
        {
            //Arrange
            int ownerId = 1;
            IQueryable<WordSuite> wordSuites = new List<WordSuite>
                                    {
                                        new WordSuite
                                        {
                                            PrototypeId = 1,
                                            OwnerId = 2
                                        }
                                    }.AsQueryable<WordSuite>();

            var mockSet = GenerateMockDbSet<WordSuite>(wordSuites);
            _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);

            //Act
            var actual = await _service.GetTeacherWordSuitesAsync(ownerId);

            //Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);

            Assert.IsEmpty(actual);
        }
        [Test]
        public void GetTeacherWordSuites_GetAllReturnsNull()
        {
            //Arrange
            int ownerId = 1;
            IQueryable<WordSuite> wordSuites = null;

            _repo.Setup(x => x.GetAll()).Returns(wordSuites);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(async () => await _service.GetTeacherWordSuitesAsync(ownerId));

            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);

        }

        [Test]
        public async Task GetWordSuitesByOwnerAndLanguageId_ReturnsListOfWordSuites()
        {
            //Arrange
            int userId = 1;
            int languageId = 1;
            IQueryable<WordSuite> expected = new List<WordSuite>
                                    {
                                        new WordSuite
                                        {
                                            Id = userId,
                                            LanguageId = languageId,
                                            PrototypeId = null,
                                            OwnerId = userId
                                        }
                                    }.AsQueryable<WordSuite>();

            var mockSet = GenerateMockDbSet<WordSuite>(expected);
            _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);

            //Act
            var actual = await _service.GetWordSuitesByOwnerAndLanguageIdAsync(userId, languageId);

            //Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);

            CollectionAssert.AreEqual(expected.ToList<WordSuite>(), actual);

        }
        [Test]
        public async Task GetWordSuitesByOwnerAndLanguageId_ReturnsEmpty()
        {
            //Arrange
            int userId = 1;
            int languageId = 1;
            IQueryable<WordSuite> wordSuites = new List<WordSuite>
                                    {
                                        new WordSuite
                                        {
                                            Id = userId,
                                            LanguageId = languageId,
                                            PrototypeId = null
                                        }
                                    }.AsQueryable<WordSuite>();

            var mockSet = GenerateMockDbSet<WordSuite>(wordSuites);
            _repo.Setup(x => x.GetAll()).Returns(mockSet.Object);

            //Act
            var actual = await _service.GetWordSuitesByOwnerAndLanguageIdAsync(userId, languageId);

            //Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);

            Assert.IsEmpty(actual);

        }
        [Test]
        public void GetWordSuitesByOwnerAndLanguageId_GetAllReturnsNull()
        {
            //Arrange
            int userId = 1;
            int languageId = 1;
            IQueryable<WordSuite> wordSuites = null;

            _repo.Setup(x => x.GetAll()).Returns(wordSuites);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(async () => await _service.GetWordSuitesByOwnerAndLanguageIdAsync(userId, languageId));

            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetAll(), Times.Once);


        }

        [Test]
        public void Add_ReturnsWordSuiteId()
        {
            //Arrange
            int expected = 1;
            WordSuite wordSuite = new WordSuite
                                        {
                                            Id = expected,
                                            LanguageId = 1
                                        };

            _repo.Setup(x => x.AddOrUpdate(It.IsAny<WordSuite>())).Verifiable();
            _uow.Setup(x => x.Save()).Verifiable();

            //Act
            var actual = _service.Add(wordSuite);

            //Assert
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _uow.Verify(x => x.Save(), Times.Once);
            _uow.VerifyAll();
            _repo.VerifyAll();

            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void Delete_ReturnsTrue()
        {
            //Arrange
            int wordSuiteId = 1;
            IQueryable<WordProgress> expected = new List<WordProgress>
                                    {
                                        new WordProgress
                                        {
                                            WordSuiteId = wordSuiteId,
                                        }
                                    }.AsQueryable<WordProgress>();
            Mock<IWordProgressRepository> _progressRepo = new Mock<IWordProgressRepository>();

            _uow.Setup(x => x.WordProgressRepository).Returns(_progressRepo.Object);
            _progressRepo.Setup(pr => pr.Delete(It.IsAny<IEnumerable<WordProgress>>())).Verifiable();
            _progressRepo.Setup(x => x.GetAll()).Returns(expected);
            _repo.Setup(r => r.Delete(It.IsAny<int>())).Verifiable();
            _uow.Setup(u => u.Save()).Verifiable();

            //Act
            var actual = _service.Delete(wordSuiteId);

            //Assert
            _uow.Verify(x => x.WordSuiteRepository, Times.Once);
            _uow.Verify(x => x.WordProgressRepository, Times.Exactly(2));
            _uow.Verify(x => x.Save(), Times.Once);
            _repo.VerifyAll();
            _uow.VerifyAll();
            _progressRepo.VerifyAll();

            Assert.IsTrue(actual);

        }
        [Test]
        public void Delete_GetAllReturnsNull()
        {
            //Arrange
            int wordSuiteId = 1;
            IQueryable<WordProgress> expected = null;
            Mock<IWordProgressRepository> _progressRepo = new Mock<IWordProgressRepository>();

            _uow.Setup(x => x.WordProgressRepository).Returns(_progressRepo.Object);
            _progressRepo.Setup(x => x.GetAll()).Returns(expected);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => _service.Delete(wordSuiteId));
            _uow.Verify(x => x.WordProgressRepository, Times.Exactly(2));

        }

        [Test]
        public void Update_ReturnsTrue()
        {
            //Arrange
            WordSuite newWordSuite = new WordSuite
            {
                Id = 1,
                Name = "name",
                Threshold = 1,
                QuizResponseTime = 1
            };

            _repo.Setup(r => r.GetById(It.IsAny<int>())).Returns(new WordSuite() { Id = 1 });
            _repo.Setup(r => r.Update(It.IsAny<WordSuite>())).Verifiable();
            _uow.Setup(u => u.Save()).Verifiable();

            //Act
            var actual = _service.Update(newWordSuite);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordSuiteRepository, Times.Exactly(2));
            _repo.VerifyAll();
            _uow.VerifyAll();

            Assert.IsTrue(actual);

        }
        [Test]
        public void Update_GetByIdNull()
        {
            //Arrange
            WordSuite newWordSuite = new WordSuite
            {
                Id = 1,
                Name = "name",
                Threshold = 1,
                QuizResponseTime = 1
            };
            WordSuite oldWordSuite = null;

            _repo.Setup(r => r.GetById(It.IsAny<int>())).Returns(oldWordSuite);

            //Act
            //Assert
            Assert.Throws<NullReferenceException>(() => _service.Update(newWordSuite));

            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordSuiteRepository, Times.Once);
            _repo.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

        }
        #endregion

        // Written by Andriy Kusyi
        //

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CopywordsuitesForTeacherByIdAsync_InvalidWordSuiteId_ArgumentExceptionExpectedThrown()
        {
            //Arrange
            int teacherId = 1;
            int wordSuiteId = 2;
            _repo.Setup(r => r.GetByIdAsync(wordSuiteId)).ReturnsAsync(null);

            //Act and Assert
            var actual = await _service.CopyWordsuitesForTeacherByIdAsync(teacherId, wordSuiteId);
        }

        [Test]
        public async Task CopywordsuitesForTeacherByIdAsync_ReturnsBool_Positive()
        {
            //Arrange
            int teacherId = 1;
            int wordSuiteId = 2;
            int wordSuiteOwnerId = 3;
            int saveAsyncPositiveResult = 1;
            Mock<IUserRepository> _usersRepo = new Mock<IUserRepository>();
            Mock<IWordProgressRepository> _wordProgressRepo = new Mock<IWordProgressRepository>();
            _uow.Setup(u => u.UserRepository).Returns(_usersRepo.Object);
            _uow.Setup(u => u.WordProgressRepository).Returns(_wordProgressRepo.Object);
            WordSuite workSuite = new WordSuite
            {
                Id = wordSuiteId,
                OwnerId = wordSuiteOwnerId
            };

            User teacherWhoShare = new User
            {
                Id = wordSuiteOwnerId
            };

            WordTranslation wordTranslation = new WordTranslation();
            IQueryable<WordProgress> wordProgresses = new List<WordProgress>
            {
                new WordProgress
                {
                    WordSuiteId = wordSuiteId,
                    WordTranslation = wordTranslation
                }
            }.AsQueryable<WordProgress>();

            var mockSet = GenerateMockDbSet<WordProgress>(wordProgresses);
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(workSuite);
            _usersRepo.Setup(u => u.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(teacherWhoShare);
            _wordProgressRepo.Setup(r => r.GetAll()).Returns(mockSet.Object);
            _wordProgressRepo.Setup(r => r.Add(It.IsAny<IEnumerable<WordProgress>>()));
            _repo.Setup(r => r.Add(It.IsAny<WordSuite>()));
            _uow.Setup(u => u.SaveAsync()).ReturnsAsync(saveAsyncPositiveResult);

            //Act
            var actual = await _service.CopyWordsuitesForTeacherByIdAsync(teacherId, wordSuiteId);
            bool expected = true;

            //Assert
            _repo.Verify(r => r.Add(It.Is<WordSuite>((ws => ws.OwnerId == teacherId))));
            _repo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _usersRepo.Verify(u => u.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _wordProgressRepo.Verify(r => r.GetAll(), Times.Once);
            _wordProgressRepo.Verify(r => r.Add(It.IsAny<IEnumerable<WordProgress>>()), Times.Once);
            _repo.Verify(r => r.Add(It.IsAny<WordSuite>()), Times.Once);
            _uow.Verify(u => u.SaveAsync(), Times.Once);
            Assert.AreEqual(actual, expected);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CopywordsuitesForTeacherListByIdAsync_InvalidWordSuiteId_ArgumentExceptionExpectedThrown()
        {
            //Arrange
            List<int> teacherIds = new List<int> { 1, 2, 3 };
            int wordSuiteId = 2;
            _repo.Setup(r => r.GetByIdAsync(wordSuiteId)).ReturnsAsync(null);

            //Act and Assert
            var actual = await _service.CopyWordsuitesForTeacherListByIdAsync(teacherIds, wordSuiteId);
        }

        [Test]
            public async Task CopywordsuitesForTeacherListByIdAsync_ReturnsBool_Positive()
            {

            //Arrange
                List<int> teacherIds = new List<int> {1, 2};
                int wordSuiteId = 2;
                int wordSuiteOwnerId = 3;
                int saveAsyncPositiveResult = 1;
                Mock<IUserRepository> _usersRepo = new Mock<IUserRepository>();
                Mock<IWordProgressRepository> _wordProgressRepo = new Mock<IWordProgressRepository>();
                _uow.Setup(u => u.UserRepository).Returns(_usersRepo.Object);
                _uow.Setup(u => u.WordProgressRepository).Returns(_wordProgressRepo.Object);

                WordSuite workSuite = new WordSuite
                {
                    Id = wordSuiteId,
                    OwnerId = wordSuiteOwnerId
                };

                User teacherWhoShare = new User
                {
                    Id = wordSuiteOwnerId
                };

                WordTranslation wordTranslation = new WordTranslation();

                IQueryable<WordProgress> wordProgresses = new List<WordProgress>
                {
                    new WordProgress
                    {
                        WordSuiteId = wordSuiteId,
                        WordTranslation = wordTranslation
                    }
                }.AsQueryable<WordProgress>();

                var mockSet = GenerateMockDbSet<WordProgress>(wordProgresses);

                _repo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(workSuite);
                _usersRepo.Setup(u => u.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(teacherWhoShare);
                _wordProgressRepo.Setup(r => r.GetAll()).Returns(mockSet.Object);

                _wordProgressRepo.Setup(r => r.Add(It.IsAny<IEnumerable<WordProgress>>()));
                _repo.Setup(r => r.Add(It.IsAny<IEnumerable<WordSuite>>()));
                _uow.Setup(u => u.SaveAsync()).ReturnsAsync(saveAsyncPositiveResult);

            //Act
                var actual = await _service.CopyWordsuitesForTeacherListByIdAsync(teacherIds, wordSuiteId);
                bool expected = true;

            //Assert
                _repo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
                _usersRepo.Verify(u => u.GetByIdAsync(It.IsAny<int>()), Times.Once);
                _wordProgressRepo.Verify(r => r.GetAll(), Times.Once);
                _wordProgressRepo.Verify(r => r.Add(It.IsAny<IEnumerable<WordProgress>>()), Times.Once);
                _repo.Verify(r => r.Add(It.IsAny<IEnumerable<WordSuite>>()), Times.Once);
                _uow.Verify(u => u.SaveAsync(), Times.Once);
                Assert.AreEqual(actual, expected);
            }
        }
    }

