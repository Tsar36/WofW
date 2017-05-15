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


namespace WorldOfWords.Tests.ServicesTests
{
    // Author : Harasymovych Yurii
    // Reviewer : Oleg Krutiy 
    [TestFixture]
    class CourseServiceTest
    {
        private Mock<IUnitOfWorkFactory> _factory;
        private Mock<IWorldOfWordsUow> _uow;
        private CourseService _service;


        [SetUp]
        public void Setup()
        {
            _factory = new Mock<IUnitOfWorkFactory>();
            _uow = new Mock<IWorldOfWordsUow>();
            _factory.Setup(x => x.GetUnitOfWork()).Returns(_uow.Object);
            _service = new CourseService(_factory.Object);
        }

        [Test]
        public void GetStudentCourses_ReturnsListOfCourses()
        {
            // Arrange
            int userId = 1;
            List<WordSuite> wordsuite = new List<WordSuite>
            {
                new WordSuite
                {
                    OwnerId = userId,
                    PrototypeId = 2
                },
                new WordSuite
                {
                    OwnerId = 2,
                    PrototypeId = null
                }
            };
            List<Course> courses = new List<Course>
                                {
                                    new Course 
                                    {
                                          Language = new Language
                                          {
                                              Id = 8
                                          },
                                          WordSuites = wordsuite

                                    }
                                };
            ICollection<Enrollment> enrollments = new List<Enrollment>
                                    {
                                        new Enrollment
                                        {
                                            Group = new Group 
                                            { 
                                                Course = courses[0]
                                                //new Course 
                                                //{
                                                //    Language = new Language
                                                //    {
                                                //        WordSuites = wordsuite,
                                                //        Id = 8
                                                //    }
                                                //}
                                            }
                                        }
                                    };
            IQueryable<User> users = new List<User>
                                    {
                                        new User
                                        {
                                            Enrollments = enrollments,
                                            Id = userId
                                        }
                                    }.AsQueryable<User>();
            Mock<IUserRepository> usersRep = new Mock<IUserRepository>();
            _factory.Setup(f => f.GetUnitOfWork()).Returns(_uow.Object);
            _uow.Setup(u => u.UserRepository).Returns(usersRep.Object);
            usersRep.Setup(u => u.GetAll()).Returns(users);
            

            //Act
            var actual = _service.GetStudentCourses(userId);

            //Assert
            _uow.Verify(x => x.UserRepository, Times.Once);
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            usersRep.Verify(x => x.GetAll(), Times.Once);
            CollectionAssert.AreEqual(courses, actual);

        }

        [Test]
        public void GetAllCourses_ReturnsListOfCourses()
        {
            // Arrange
            int userId=8;
            IQueryable<Course> expected = new List<Course>
            {
                new Course
                {
                    WordSuites = new List<WordSuite>(), 
                    Language = new Language(),
                    OwnerId=8
                   
                }
            }.AsQueryable<Course>();
            Mock<IRepository<WorldOfWords.Domain.Models.Course>> repo = new Mock<IRepository<Course>>();
            _uow.Setup(x => x.CourseRepository).Returns(repo.Object);
            repo.Setup(x => x.GetAll()).Returns(expected);

            // Act
            var actual = _service.GetAllCourses(userId);
            
            // Assert
            _uow.Verify(x => x.CourseRepository, Times.Once);
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            repo.Verify(x => x.GetAll(), Times.Once);
            CollectionAssert.AreEqual(expected, actual);


        }


        [Test]
        public void GetTeacherCourses_ReturnCourses()
        {
            // Arrange
            int userId = 8;
            Course expected = new Course
            {
                OwnerId = userId,
                WordSuites = new List<WordSuite>()
                    { 
                        new WordSuite
                        {

                            PrototypeId = null
                        }
                    },
                Language = new Language(),
               
            };
            IQueryable<Course> courses = new List<Course>
            {
                expected
            }.AsQueryable<Course>();
            Mock<IRepository<WorldOfWords.Domain.Models.Course>> repo = new Mock<IRepository<Course>>();
            _uow.Setup(x => x.CourseRepository).Returns(repo.Object);
            repo.Setup(x => x.GetAll()).Returns(courses);
            
            //Act
            var actual = _service.GetTeacherCourses(userId);

            //Assert
            _uow.Verify(x => x.CourseRepository, Times.Once);
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            repo.Verify(x => x.GetAll(), Times.Once);
            Assert.AreEqual(courses.ToList<Course>(), actual);
        }
       
        [Test]
        public void Add_ReturnsId()
        {

            // Arrange
            int id = 322;
            Course course = new Course
            {
                Id = 322,
                WordSuites = new List<WordSuite>()
            };
            List<int> list = new List<int>()
            {
                1,2,3,4,322
            };
            Mock<IRepository<WorldOfWords.Domain.Models.Course>> repo = new Mock<IRepository<Course>>();
            _uow.Setup(x => x.CourseRepository).Returns(repo.Object);
            _uow.Setup(x => x.CourseRepository.GetById(It.IsAny<int>())).Returns(course);
            Mock<ICollection<WordSuite>> WordSuites = new Mock<ICollection<WordSuite>>();
            Mock<IWordSuiteRepository> wordSuiteRepository = new Mock<IWordSuiteRepository>();
            _uow.Setup(x => x.WordSuiteRepository).Returns(wordSuiteRepository.Object);
            wordSuiteRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(It.IsAny<WordSuite>());


            // Act
            var actual = _service.Add(course, list);
            
            // Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Exactly(2));
            _uow.Verify(x => x.CourseRepository.GetById(course.Id), Times.AtLeastOnce());
            _uow.Verify(x => x.Save(), Times.Exactly(3));
            _uow.Verify(x => x.CourseRepository, Times.Exactly(2));
            _uow.Verify(x => x.CourseRepository.Add(course), Times.Once);
            //WordSuites.Verify(x=>x.Add(),Times.AtLeastOnce);
            Assert.AreEqual(id, actual);
            
        }

        [Test]
        public void Delete_CourseById()
        {
           
            // Arrange
            Mock<IRepository<WorldOfWords.Domain.Models.Course>> repo = new Mock<IRepository<Course>>();
            _uow.Setup(x => x.CourseRepository).Returns(repo.Object);
            repo.Setup(x => x.Delete(It.IsAny<int>())).Verifiable();
            _uow.Setup(x => x.Save()).Verifiable();

            // Act
            _service.Delete(It.IsAny<int>());

            // Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.CourseRepository,Times.Once);
            repo.Verify(x => x.Delete(It.IsAny<int>()), Times.Once);
            _uow.Verify(x => x.Save(), Times.Once());
            repo.VerifyAll();
            _uow.VerifyAll();
        }

        [Test]
        public void GetById_ReturnCourse_Positive()
        {
           // Arrange 
            int id = 8;
            Course expected = new Course
                {
                    WordSuites = new List<WordSuite>()
                    { 
                        new WordSuite
                        {
                            PrototypeId =null
                        },
                        new WordSuite
                        {
                          PrototypeId = 2  
                        }
                    },
                    Language = new Language(),
                    Groups = new List<Group>(),
                    Id = 8
                };
            IQueryable<Course> courses = new List<Course>
            {
                expected
            }.AsQueryable<Course>();
            Mock<IRepository<WorldOfWords.Domain.Models.Course>> repo = new Mock<IRepository<Course>>();
            _uow.Setup(x => x.CourseRepository).Returns(repo.Object);
            repo.Setup(x => x.GetAll()).Returns(courses);

            //Act
            var actual = _service.GetById(id);

            //Assert
            _uow.Verify(x => x.CourseRepository, Times.Once);
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            repo.Verify(x => x.GetAll(), Times.Once);
            Assert.AreEqual(expected, actual);
            
        }

        [Test]
        public void GetById_ReturnCourse_Negative()
        {
            // Arrange 
            int id = 8;
            Course expected = new Course
            {
                
                Id = 322
            };
            IQueryable<Course> courses = new List<Course>
            {
                expected
            }.AsQueryable<Course>();
            Mock<IRepository<WorldOfWords.Domain.Models.Course>> repo = new Mock<IRepository<Course>>();
            _uow.Setup(x => x.CourseRepository).Returns(repo.Object);
            repo.Setup(x => x.GetAll()).Returns(courses);

            //Act
            var actual = _service.GetById(id);

            //Assert
            _uow.Verify(x => x.CourseRepository, Times.Once);
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            repo.Verify(x => x.GetAll(), Times.Once);
            Assert.IsNull(actual);
        }

        [Test]
        public void GetById_ReturnCourse_Overloaded()
        {
            // Arrange
            int id = 21;
            int userId =8;
            Course expected = new Course
            {
                WordSuites = new List<WordSuite>()
                    { 
                        new WordSuite
                        {
                            OwnerId = userId,
                            PrototypeId = 8
                        },
                        new WordSuite
                        {
                            OwnerId = 1,
                            PrototypeId = 2
                        }
                    },
                Language = new Language(),
                Id = id
            };
            IQueryable<Course> courses = new List<Course>
            {
                expected
            }.AsQueryable<Course>();
            Mock<IRepository<WorldOfWords.Domain.Models.Course>> repo = new Mock<IRepository<Course>>();
            _uow.Setup(x => x.CourseRepository).Returns(repo.Object);
            repo.Setup(x => x.GetAll()).Returns(courses);
            
            // Act
            var actual = _service.GetById(id, userId);

            // Assert 
            _uow.Verify(x => x.CourseRepository, Times.Once);
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            repo.Verify(x => x.GetAll(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetProgress_ReturnProgress()
        {
            // Arrange
            double expected = 600;
            int userId = 10;
            int id = 8;
            Course course = new Course
            {
                WordSuites = new List<WordSuite>()
                    { 
                        new WordSuite
                        {
                            
                            OwnerId = userId,
                            WordProgresses = new List<WordProgress>
                            {
                                new WordProgress
                                {
                                    Progress = 10

                                }
                            },
                            Threshold = 1,
                            PrototypeId = 2
                        },
                        new WordSuite
                        {
                            OwnerId = 10,
                            PrototypeId = 10,
                            WordProgresses = new List<WordProgress>
                            {
                                new WordProgress
                                {
                                    Progress = 2
                                }
                            },
                            Threshold = 1
                        },
                    },
                Language = new Language(),
                Groups = new List<Group>(),
                Id = 8
            };
            IQueryable<Course> courses = new List<Course>
            {
                course
            }.AsQueryable<Course>();
            Mock<IRepository<WorldOfWords.Domain.Models.Course>> repo = new Mock<IRepository<Course>>();
            _uow.Setup(x => x.CourseRepository).Returns(repo.Object);
            repo.Setup(x => x.GetAll()).Returns(courses);
            
        
            // Act
            double actual = _service.GetProgress(id, userId);

            // Assert
            _factory.Verify(x => x.GetUnitOfWork(), Times.Once);
            _uow.Verify(x => x.CourseRepository, Times.Once);
            repo.Verify(x => x.GetAll(), Times.Once);
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void EditCourse()
        {   
            //Arrange
            List<int> wordSuiteId = new List<int>()
            {
                1,2,3,4
            };

            List<WordSuite> wordsuite = new List<WordSuite>
            {
                new WordSuite
                {
                  Id = 8,
                  PrototypeId = null
                },
                new WordSuite
                {
                    Id = 500,
                    PrototypeId = 100 
                }
            };
            Course courseModel = new Course
            {
                Name = "eke",
                Id = 1
            };
            Course course = new Course
            {
                WordSuites = wordsuite,
                Id = 322
            };
            Mock<IRepository<WorldOfWords.Domain.Models.Course>> repo = new Mock<IRepository<Course>>();
            _uow.Setup(x => x.CourseRepository).Returns(repo.Object);
            //  Mock<IRepository<Course>> _repCourse = new Mock<IRepository<Course>>();
            _uow.Setup(x => x.Save()).Verifiable();
            repo.Setup(x => x.GetById(It.IsAny<int>())).Returns(course);
            Mock<IWordSuiteRepository> suiteRepo = new Mock<IWordSuiteRepository>();
            _uow.Setup(x => x.WordSuiteRepository).Returns(suiteRepo.Object);
            suiteRepo.Setup(x => x.GetById(It.IsAny<int>())).Returns(It.IsAny<WordSuite>());

            //Act
            _service.EditCourse(courseModel, wordSuiteId);
            
            //Assert
            _uow.VerifyAll();
            _uow.Verify(x=>x.Save(),Times.Exactly(3));
            _factory.Verify(x=>x.GetUnitOfWork(),Times.Exactly(3));
            _uow.Verify(x => x.CourseRepository, Times.Exactly(3));
            repo.Verify(x => x.GetById(It.IsAny<int>()), Times.Exactly(3));
            _uow.Verify(x => x.WordSuiteRepository, Times.AtLeastOnce());
            suiteRepo.Verify(x => x.GetById(It.IsAny<int>()), Times.AtLeastOnce());
            _uow.VerifyAll();
        }
    }
}
