//Oleh Krutii
//Reviwer: Harasymovych Yurii
//Evgenii Khomutsky
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NUnit.Framework;
using Moq;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.Infrastructure.Data.EF.UnitOfWork;
using WorldOfWords.Domain.Models;
using WorldOfWords.API.Models;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using WorldOfWords.Domain.Services; 
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using WorldOfWords.API.Models.Mappers;

namespace WorldOfWords.Tests.ServicesTests
{
    [TestFixture]
    class TagServiceTest
    {
        private Mock<IUnitOfWorkFactory> _factory;
        private Mock<IWorldOfWordsUow> _uow;
        private Mock<IRepository<Tag>> _repo;
        private TagService _service;
        private Mock<ITagMapper> _mapper;

        [SetUp]
        public void Setup()
        {
            _mapper = new Mock<ITagMapper>();
            _factory = new Mock<IUnitOfWorkFactory>();
            _uow = new Mock<IWorldOfWordsUow>();
            _repo = new Mock<IRepository<Tag>>();
            _factory.Setup(f => f.GetUnitOfWork()).Returns(_uow.Object);
            _uow.Setup(u => u.TagRepository).Returns(_repo.Object);
            _service = new TagService(_factory.Object, _mapper.Object);
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
        public void Exists_ReturnsTagId_TagExists()
        {
            //Arrange
            string name = "pesik";
            int expected = 8;
            var listik = new List<Tag>
            {
                new Tag 
                {
                    Name = name,
                    Id = expected
                }
            }.AsQueryable<Tag>();
            _repo.Setup(r => r.GetAll()).Returns(listik);

            //Act
             int actual = _service.Exists(name);

            //Assert
             _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
             _uow.Verify(u => u.TagRepository, Times.Once);
             _repo.Verify(r => r.GetAll(), Times.Once);
             Assert.AreEqual(expected, actual);

        }

        [Test]
        public void Exists_ReturnsZero_TagNotExist()
        {
            //Arrange
            string name = "pesik";
            int expected = 0;
            var listik = new List<Tag>
            {
                new Tag 
                {
                    Name = name + "failed",
                    Id = expected
                }
            }.AsQueryable<Tag>();
            _repo.Setup(r => r.GetAll()).Returns(listik);

            //Act
            int actual = _service.Exists(name);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.TagRepository, Times.Once);
            _repo.Verify(r => r.GetAll(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        async public void GetTagByIdAsync_ReturnsTag()
        {
            //Arrange
            int testId = 2;
            var tag = new Tag { Id = testId };
            
            _repo.Setup(r => r.GetByIdAsync(testId)).ReturnsAsync(tag);
           
            //Act
            Tag actual = await _service.GetTagByIdAsync(testId);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.TagRepository, Times.Once);
            _repo.Verify(r => r.GetByIdAsync(It.Is<int>(s => s == testId)), Times.Once);
            
            Assert.AreEqual(tag, actual);
        }
        
        [Test]
        async public void GetFirstTagByNameAsync_ReturnTag_Exist()
        {
            //Arrange
            string tagName = "Test";
            var expected = new List<Tag>()
            {
                new Tag { Name = tagName }
            
            }.AsQueryable();
            
            var tagDB = GenerateMockDbSet<Tag>(expected);
            _repo.Setup(r => r.GetAll()).Returns(tagDB.Object);
            
            //Act
            var actual = await _service.GetFirstTagByNameAsync(tagName);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.TagRepository, Times.Once);
            _repo.Verify(r => r.GetAll(), Times.Once);
            Assert.AreEqual(expected.First(), actual);
        }

        [Test]
        async public void GetFirstTagByNameAsync_ReturnNull_Default()
        {
            //Arrange
            var list = new List<Tag>() {}.AsQueryable();

            var tagDB = GenerateMockDbSet<Tag>(list);
            _repo.Setup(r => r.GetAll()).Returns(tagDB.Object);

            //Act
            var actual = await _service.GetFirstTagByNameAsync("");

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.TagRepository, Times.Once);
            _repo.Verify(r => r.GetAll(), Times.Once);
            Assert.IsNull(actual);
        }

        [Test]
        async public void AddAsync_TagDontExist_ReturnsID()
        {
            //Arrange
            int expected = 1;
            var model = new TagModel() { };
            _mapper.Setup(m => m.Map(It.IsAny<TagModel>())).Returns(new Tag() {Id = 1});
            _repo.Setup(r => r.Add(It.IsAny<Tag>())).Verifiable();
            _uow.Setup(u => u.SaveAsync()).ReturnsAsync(expected);

            //Act
            var actual = await _service.AddAsync(model);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Exactly(2));
            _uow.Verify(u => u.TagRepository, Times.Exactly(2));
            _repo.VerifyAll();
            _mapper.Verify(m => m.Map(It.IsAny<TagModel>()), Times.Once);
            _uow.Verify(u => u.SaveAsync(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        async public void AddAsync_TagExists_ReturnsMinusOne()
        {
            //Arrange
            int expected = -1;
            var list = new List<Tag>() { new Tag() { Id = 100, Name = "Test" } }.AsQueryable();
            var test = new TagModel() { Value = "Test" };
            
            _repo.Setup(r => r.GetAll()).Returns(list);

            //Act
            var actual = await _service.AddAsync(test);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Exactly(2));
            _uow.Verify(u => u.TagRepository, Times.Once);
            _repo.Verify(r => r.GetAll(), Times.Once);
            _mapper.Verify(m => m.Map(It.IsAny<TagModel>()), Times.Never);
            _uow.Verify(u => u.SaveAsync(), Times.Never);
            Assert.AreEqual(expected, actual);
        }
    }
}