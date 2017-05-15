using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using WorldOfWords.Domain.Services.Services;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.Infrastructure.Data.EF.UnitOfWork;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services;

namespace WorldOfWords.Tests.ServicesTests
{
    [TestFixture]
    class WordProgressServiceTest
    {
        private Mock<IUnitOfWorkFactory> _factory;
        private Mock<IWorldOfWordsUow> _uow;
        private Mock<IWordProgressRepository> _progressRepository;


        [SetUp]
        public void Setup()
        {
            _factory = new Mock<IUnitOfWorkFactory>();
            _uow = new Mock<IWorldOfWordsUow>();
            _progressRepository = new Mock<IWordProgressRepository>();

            _factory.Setup(f => f.GetUnitOfWork()).Returns(_uow.Object);
        }

        [Test]
        public void IsStudentWord_True()
        {
            //Arrange
            WordProgress _wordProgress = new WordProgress
            {
                IsStudentWord = true,
                Progress = 0,
                WordSuite = null,
                WordSuiteId = 3,
                WordTranslation = null,
                WordTranslationId = 3

            };

            IQueryable<WordProgress> _queryable = new List<WordProgress>
            {
               _wordProgress
            }.AsQueryable();

            _uow.Setup(u => u.WordProgressRepository).Returns(_progressRepository.Object);
            _progressRepository.Setup(p => p.GetAll()).Returns(_queryable);

            //Act
            var result = new WordProgressService(_factory.Object).IsStudentWord(_wordProgress);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordProgressRepository, Times.Once);
            _progressRepository.Verify(p => p.GetAll(), Times.Exactly(1));
            Assert.IsTrue(result, "IsStudentWord");

        }
        [Test]
        public void IsStudentWord_False()
        {
            //Arrange
            WordProgress _wordProgress = new WordProgress
            {
                IsStudentWord = false,
                Progress = 0,
                WordSuite = null,
                WordSuiteId = 3,
                WordTranslation = null,
                WordTranslationId = 3

            };

            IQueryable<WordProgress> _queryable = new List<WordProgress>
            {
               _wordProgress
            }.AsQueryable();
            _uow.Setup(u => u.WordProgressRepository).Returns(_progressRepository.Object);
            _progressRepository.Setup(p => p.GetAll()).Returns(_queryable);

            //Act
            var result = new WordProgressService(_factory.Object).IsStudentWord(_wordProgress);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordProgressRepository, Times.Once);
            _progressRepository.Verify(p => p.GetAll(), Times.Exactly(1));
            Assert.IsFalse(result, "IsNotStudentWord");

        }
        [Test]
        public void IncrementProgress_True()
        {
            //Arrange
            WordProgress _wordProgress = new WordProgress()
            {
                IsStudentWord = false,
                Progress = 4,
                WordSuite = null,
                WordSuiteId = 3,
                WordTranslation = null,
                WordTranslationId = 3
            };
            IQueryable<WordProgress> _queryable = new List<WordProgress>
            {
               _wordProgress
            }.AsQueryable();

            _uow.Setup(u => u.WordProgressRepository).Returns(_progressRepository.Object);
            _progressRepository.Setup(p => p.GetAll()).Returns(_queryable);
            _progressRepository.Setup(p => p.AddOrUpdate(_wordProgress));
            _uow.Setup(u => u.Save());

            //Act
            var result = new WordProgressService(_factory.Object).IncrementProgress(_wordProgress.WordSuiteId, _wordProgress.WordTranslationId);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordProgressRepository, Times.Exactly(2));
            _progressRepository.Verify(p => p.GetAll(), Times.Exactly(1));
            _progressRepository.Verify(p => p.AddOrUpdate(_wordProgress), Times.Exactly(1));
            _uow.Verify(u => u.Save(), Times.Once);
            //if IncrementProgress returns true, _wordProgress.Progress=4+1=5
            Assert.AreEqual(_wordProgress.Progress, 5);
            Assert.IsTrue(result, "true");

        }

        [Test]
        public void RemoveProgressesForEnrollment_Removed()
        {
            //Arrange
            int _enrollmentId = 1;

            List<WordSuite> _collection = new List<WordSuite>() 
            {
                new WordSuite(){Id = 1, PrototypeId = null},
                new WordSuite()
                {OwnerId = 3, PrototypeId = 1, 
                    WordProgresses = new List<WordProgress>()
                    {
                        new WordProgress()
                    }
                }
            };
            Enrollment _enrollment = new Enrollment()
            {
                Date = System.DateTime.Now,
                Group = new Group()
                {
                    Course = new Course()
                    {
                        WordSuites = _collection
                    }
                },
                GroupId = 0,
                Id = _enrollmentId,
                User = new User()
                {
                    Id = 3
                },
                UserId = 0
            };


            Mock<IRepository<Enrollment>> _enrollmentRepository;
            _enrollmentRepository = new Mock<IRepository<Enrollment>>();

            _uow.Setup(u => u.EnrollmentRepository).Returns(_enrollmentRepository.Object);
            _uow.Setup(u => u.WordProgressRepository).Returns(_progressRepository.Object);
            _uow.Setup(u => u.Save());
            _enrollmentRepository.Setup(e => e.GetById(_enrollmentId)).Returns(_enrollment);
            _progressRepository.Setup(p => p.Delete(It.IsAny<IEnumerable<WordProgress>>()));
            var _service = new WordProgressService(_factory.Object);

            //Act
            _service.RemoveProgressesForEnrollment(_enrollmentId);


            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.EnrollmentRepository, Times.Exactly(1));
            _uow.Verify(u => u.WordProgressRepository, Times.Exactly(1));
            _enrollmentRepository.Verify(e => e.GetById(_enrollmentId), Times.Exactly(1));
            _progressRepository.Verify(p => p.Delete(It.IsAny<IEnumerable<WordProgress>>()), Times.Once);
            _uow.Verify(u => u.Save(), Times.Once);





        }

        [Test]
        public void RemoveProgressesForEnrollment_Exception()
        {
            //Arrange
            int _enrollmentId = 1;
            string _message = "";
            Enrollment _enrollment = null;

            Mock<IRepository<Enrollment>> _enrollmentRepository;
            _enrollmentRepository = new Mock<IRepository<Enrollment>>();

            _uow.Setup(u => u.EnrollmentRepository).Returns(_enrollmentRepository.Object);
            _uow.Setup(u => u.WordProgressRepository).Returns(_progressRepository.Object);
            _uow.Setup(u => u.Save());
            _enrollmentRepository.Setup(e => e.GetById(_enrollmentId)).Returns(_enrollment);
            _progressRepository.Setup(p => p.Delete(It.IsAny<IEnumerable<WordProgress>>()));
            var _service = new WordProgressService(_factory.Object);

            //Act 

            try
            {
                _service.RemoveProgressesForEnrollment(_enrollmentId);
            }
            catch (ArgumentException ex)
            {
                _message = ex.Message;
            }

            //Assert
            Assert.AreEqual("Enrollment with id you are requesting does not exist", _message);



            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.EnrollmentRepository, Times.Exactly(1));
            _uow.Verify(u => u.WordProgressRepository, Times.Never);
            _enrollmentRepository.Verify(e => e.GetById(_enrollmentId), Times.Exactly(1));
            _progressRepository.Verify(p => p.Delete(It.IsAny<IEnumerable<WordProgress>>()), Times.Never);
            _uow.Verify(u => u.Save(), Times.Never);




        }

        [Test]
        public void CopyProgressesForUsersInGroup_Copied()
        {
            //Arrange
            int _groupId = 1;
            List<WordSuite> _collection = new List<WordSuite>() 
            {
                new WordSuite(){Id = 1, PrototypeId = null},
                new WordSuite()
                {OwnerId = 3, PrototypeId = 1, 
                    PrototypeWordSuite=new WordSuite()
                    {
                         WordProgresses=new List<WordProgress>()
                    },
                    WordProgresses = new List<WordProgress>()
                    {
                        new WordProgress()
                    }
                }
            };
            Group _group = new Group()
            {
                Course = new Course()
                {
                    WordSuites = _collection
                }
            };
            var _users = new List<User>() 
            {
                new User()
                {
                    Id = 3
                }
            };
            var _progressesToAdd = new List<WordProgress>();
            var _groupRepository = new Mock<IRepository<WorldOfWords.Domain.Models.Group>>();
            _uow.Setup(u => u.GroupRepository).Returns(_groupRepository.Object);
            _groupRepository.Setup(g => g.GetById(_groupId)).Returns(_group);
            _uow.Setup(u => u.WordProgressRepository).Returns(_progressRepository.Object);
            _uow.Setup(u => u.Save());
            _progressRepository.Setup(p => p.Add(It.IsAny<IEnumerable<WordProgress>>()));

            var _service = new WordProgressService(_factory.Object);

            //Act
            _service.CopyProgressesForUsersInGroup(_users, _groupId);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.GroupRepository, Times.Exactly(1));
            _uow.Verify(u => u.WordProgressRepository, Times.Exactly(1));
            _groupRepository.Verify(e => e.GetById(_groupId), Times.Exactly(1));
            _progressRepository.Verify(p => p.Add(It.IsAny<IEnumerable<WordProgress>>()), Times.Once);
            _uow.Verify(u => u.Save(), Times.Once);

        }

        [Test]
        public void CopyProgressesForUsersInGroup_Exception()
        {
            //Arrange
            int _groupId = 1;
            string _message = "";
            Group _group = null;
            var _users = new List<User>() 
            {
                new User()
                {
                    Id = 3
                }
            };
            var _groupRepository = new Mock<IRepository<WorldOfWords.Domain.Models.Group>>();
            _uow.Setup(u => u.GroupRepository).Returns(_groupRepository.Object);
            _groupRepository.Setup(g => g.GetById(_groupId)).Returns(_group);


            var _service = new WordProgressService(_factory.Object);

            //Act
            try
            {
                _service.CopyProgressesForUsersInGroup(_users, _groupId);
            }
            catch (ArgumentException ex)
            {
                _message = ex.Message;
            }

            //Assert
            Assert.AreEqual("Group with id you are requesting does not exist", _message);
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.GroupRepository, Times.Exactly(1));
            _uow.Verify(u => u.WordProgressRepository, Times.Never);
            _groupRepository.Verify(g => g.GetById(_groupId), Times.Exactly(1));
            _progressRepository.Verify(p => p.Add(It.IsAny<IEnumerable<WordProgress>>()), Times.Never);
            _uow.Verify(u => u.Save(), Times.Never);

        }

        [Test]
        public void AddRange_True()
        {
            //Arrange
            var _wordProgressRange = new List<WordProgress>()
            {
                new WordProgress()
            };

            _uow.Setup(u => u.WordProgressRepository).Returns(_progressRepository.Object);
            _progressRepository.Setup(p => p.AddOrUpdate(It.IsAny<WordProgress>()));
            _uow.Setup(u => u.Save());
            var _service = new WordProgressService(_factory.Object);

            //Act
            var result = _service.AddRange(_wordProgressRange);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordProgressRepository, Times.Once);
            _progressRepository.Verify(p => p.AddOrUpdate(It.IsAny<WordProgress>()), Times.Once);
            _uow.Verify(u => u.Save(), Times.Once);
            Assert.IsTrue(result, "true");

        }

        [Test]
        public void RemoveRange_True()
        {
            //Arrange
            var _wordProgressRange = new List<WordProgress>()
            {
                new WordProgress()
                {
                    WordSuiteId = 7,
                    WordTranslationId = 8
                },
                new WordProgress()
                {
                    WordSuiteId = 7,
                    WordTranslationId = 8
                }
            };
            var _wordProgress = new WordProgress()
            {
                WordSuiteId = 7,
                WordTranslationId = 8
            };
            IQueryable<WordProgress> _queryable = new List<WordProgress>
            {
               _wordProgress
            }.AsQueryable();
            _uow.Setup(u => u.WordProgressRepository).Returns(_progressRepository.Object);
            _progressRepository.Setup(p => p.Delete(It.IsAny<WordProgress>()));
            _progressRepository.Setup(p => p.GetAll()).Returns(_queryable);
            _uow.Setup(u => u.Save());
            var _service = new WordProgressService(_factory.Object);

            //Act
            var result = _service.RemoveRange(_wordProgressRange);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordProgressRepository, Times.Exactly(4));
            _progressRepository.Verify(p => p.Delete(It.IsAny<WordProgress>()), Times.Exactly(2));
            _progressRepository.Verify(p => p.GetAll(), Times.Exactly(2));
            _uow.Verify(u => u.Save(), Times.Once);
            Assert.IsTrue(result, "true");

        }

        [Test]
        public void RemoveByStudent_True()
        {
            //Arrange
            var _wordProgress = new WordProgress()
            {
                IsStudentWord = true,
                WordSuiteId = 5,
                WordTranslationId = 5
            };
            IQueryable<WordProgress> _queryable = new List<WordProgress>
            {
               _wordProgress
            }.AsQueryable();
            _uow.Setup(u => u.WordProgressRepository).Returns(_progressRepository.Object);
            _progressRepository.Setup(p => p.GetAll()).Returns(_queryable);
            _progressRepository.Setup(p => p.Delete(It.IsAny<WordProgress>()));
            _uow.Setup(u => u.Save());

            //Act
            var result = new WordProgressService(_factory.Object).RemoveByStudent(_wordProgress);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Exactly(2));
            _progressRepository.Verify(p => p.GetAll(), Times.Exactly(2));
            _progressRepository.Verify(p => p.Delete(_wordProgress), Times.Exactly(1));
            _uow.Verify(u => u.Save(), Times.Once);
            Assert.IsTrue(result, "true");
        }

        [Test]
        public void RemoveByStudent_False()
        {
            //Arrange
            var _wordProgress = new WordProgress()
            {
                IsStudentWord = false,
                WordSuiteId = 5,
                WordTranslationId = 5
            };
            IQueryable<WordProgress> _queryable = new List<WordProgress>
            {
               _wordProgress
            }.AsQueryable();
            _uow.Setup(u => u.WordProgressRepository).Returns(_progressRepository.Object);
            _progressRepository.Setup(p => p.GetAll()).Returns(_queryable);
            _progressRepository.Setup(p => p.Delete(It.IsAny<WordProgress>()));
            _uow.Setup(u => u.Save());

            //Act
            var result = new WordProgressService(_factory.Object).RemoveByStudent(_wordProgress);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Exactly(2));
            _progressRepository.Verify(p => p.GetAll(), Times.Exactly(1));
            _progressRepository.Verify(p => p.Delete(_wordProgress), Times.Exactly(0));
            _uow.Verify(u => u.Save(), Times.Never);
            Assert.IsFalse(result, "false");
        }


        // Author : Roman Prytulenko
        // Reviewer : 
        [Test]
        public void IncrementProgressOfOriginalWord_True()
        {
            //Arrange
            int wordSuiteId = 5;
            string originalWord = "test";

            Word _word = new Word()
            {
                Value = "test"
            };

            WordTranslation _wordTranslation = new WordTranslation()
            {
                OriginalWordId = 2,
                OriginalWord = _word,
            };

            WordProgress _wordProgress = new WordProgress()
            {
                IsStudentWord = false,
                WordTranslationId = 2,
                Progress = 4,
                WordSuite = null,
                WordSuiteId = 5,
                WordTranslation = _wordTranslation
            };
            IQueryable<WordProgress> _queryable = new List<WordProgress>
            {
               _wordProgress
            }.AsQueryable();

            _uow.Setup(u => u.WordProgressRepository).Returns(_progressRepository.Object);
            _progressRepository.Setup(p => p.GetAll()).Returns(_queryable);
            _progressRepository.Setup(p => p.AddOrUpdate(_wordProgress));
            _uow.Setup(u => u.Save()).Verifiable();

            //Act
            bool result = new WordProgressService(_factory.Object).IncrementProgressOfOriginalWord(wordSuiteId, originalWord);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.WordProgressRepository, Times.Exactly(2));
            _progressRepository.Verify(p => p.GetAll(), Times.Exactly(1));
            _progressRepository.Verify(p => p.AddOrUpdate(_wordProgress), Times.Exactly(1));
            _uow.Verify(u => u.Save(), Times.Once);
            _uow.VerifyAll();
            Assert.IsTrue(result);
        }

        //[Test]
        //public void RemoveProgressesForEnrollment_EnrollmentIsNull()
        //{

        //}
    }
}
