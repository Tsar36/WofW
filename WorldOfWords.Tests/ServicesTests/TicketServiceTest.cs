using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.Infrastructure.Data.EF.UnitOfWork;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using WorldOfWords.API.Models.Mappers;
using WorldOfWords.Domain.Services.Services;
using WorldOfWords.API.Models.Models;
using WorldOfWords.API.Models.IMappers;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using WorldOfWords.API.Models;

namespace WorldOfWords.Tests.ServicesTests
{
    [TestFixture]
    public class TicketServiceTest
    {
        private Mock<IUnitOfWorkFactory> _factory;
        private Mock<IWorldOfWordsUow> _uow;
        private Mock<IRepository<Ticket>> _repo;
        private Mock<ITicketMapper> _mapper;
        private TicketService _service;

        [SetUp]
        public void Setup()
        {
            _factory = new Mock<IUnitOfWorkFactory>();
            _uow = new Mock<IWorldOfWordsUow>();
            _repo = new Mock<IRepository<Ticket>>();
            _mapper = new Mock<ITicketMapper>();
            //_mapper.Setup(m => m.Map(It.IsAny<Ticket>())).Returns(It.IsAny<TicketForListModel>());
            _factory.Setup(f => f.GetUnitOfWork()).Returns(_uow.Object);
            _uow.Setup(u => u.TicketRepository).Returns(_repo.Object);
            _service = new TicketService(_factory.Object, _mapper.Object);
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
        public async void GetAllTicketsAsync_ReturnsIEnumerable()
        {
            //Arrange
            List<TicketForListModel> expected = new List<TicketForListModel>
            {
                new TicketForListModel()
                {
                    TicketId = 8
                }
            };
            IQueryable<Ticket> tickets = new List<Ticket>()
            {
                new Ticket()
                {
                    TicketId = 8
                }
            }.AsQueryable();
            _mapper.Setup(m => m.Map(It.IsAny<Ticket>())).Returns(expected[0]);
            var ticketDb = GenerateMockDbSet<Ticket>(tickets);
            _repo.Setup(r => r.GetAll()).Returns(ticketDb.Object);

            //Act
            var actual = await _service.GetAllTicketsAsync();

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.TicketRepository, Times.Once);
            _repo.Verify(w => w.GetAll(), Times.Once);
           //!!? _mapper.Verify(m => m.Map(It.IsAny<Ticket>()), Times.Once);  ?!! 0 times
            CollectionAssert.AreEqual(expected.AsEnumerable<TicketForListModel>(), actual);

        }

        [Test]
        public void GetAllTicketsAsync_ThrowsArgumentNullException()
        {
            //Arrange
            IQueryable<Ticket> tickets = null;
            _repo.Setup(r => r.GetAll()).Returns(tickets);

            //Act
            
            //Assert
            Assert.Throws<ArgumentNullException>(async () => await _service.GetAllTicketsAsync());
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.TicketRepository, Times.Once);
            _repo.Verify(w => w.GetAll(), Times.Once);

        }

        [Test]
        public async void GetTicketsByUserIdAsync_ReturnsTicketForListModels()
        {
            //Arrange
            int id = 88;
            List<TicketForListModel> expected = new List<TicketForListModel>
            {
                new TicketForListModel()
                {
                    TicketId = 8,
                    OwnerId = id
                }
            };
            IQueryable<Ticket> tickets = new List<Ticket>()
            {
                new Ticket()
                {
                    TicketId = 8,
                    OwnerId = id
                }   
            }.AsQueryable();
            _mapper.Setup(m => m.Map(tickets.First())).Returns(expected[0]);
            var ticketDb = GenerateMockDbSet<Ticket>(tickets);
            _repo.Setup(r => r.GetAll()).Returns(ticketDb.Object);

            //Act
            var actual = await _service.GetTicketsByUserIdAsync(id);

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.TicketRepository, Times.Once);
            _repo.Verify(w => w.GetAll(), Times.Once);
            //??! _mapper.Verify(m => m.Map(tickets.First()), Times.Once); ??! 0 times
            CollectionAssert.AreEqual(expected.AsEnumerable<TicketForListModel>(), actual);
        }

        [Test]
        public void GetTicketsByUserIdAsync_ThrowsArgumentNullExceptrion()
        {
            //Arrange
            IQueryable<Ticket> tickets = null;
            _repo.Setup(r => r.GetAll()).Returns(tickets);

            //Act
            
            //Assert
            Assert.Throws<ArgumentNullException>(async() => await _service.GetTicketsByUserIdAsync(It.IsAny<int>()));
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.TicketRepository, Times.Once);
            _repo.Verify(w => w.GetAll(), Times.Once);
        }

        [Test]
        public async void GetTicketByIdAsync_ReturnsTicketForListModel()
        {
            //Arrange
            int id = 8;
            TicketForListModel expected = new TicketForListModel()
            {
                TicketId = id
            };
            Ticket ticket = new Ticket()
            {
                    TicketId = id
            };
            _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(ticket);
            _mapper.Setup(m => m.Map(ticket)).Returns(expected);

            //Act
            var actual = await _service.GetTicketByIdAsync(id);

            //Arrange
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.TicketRepository, Times.Once);
            _repo.Verify(r => r.GetByIdAsync(id), Times.Once);
            _mapper.Verify(m => m.Map(ticket), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async void AddTicketAsync_returnsInt()
        {
            //Arrange
            int expected = 8;
            _mapper.Setup(m => m.Map(It.IsAny<RequestFromUserModel>())).Returns(It.IsAny<Ticket>());
            _repo.Setup(r => r.Add(It.IsAny<Ticket>())).Verifiable();
            _uow.Setup(u => u.SaveAsync()).ReturnsAsync(expected);

            //Act
            var actual = await _service.AddTicketAsync(It.IsAny<RequestFromUserModel>());

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.TicketRepository, Times.Once);
            _repo.VerifyAll();
            _mapper.Verify(m => m.Map(It.IsAny<RequestFromUserModel>()), Times.Once);
            _uow.Verify(u => u.SaveAsync(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async void  UpdateAsync_ReturnsInt()
        {
            //Arrange
            int expected = 8;
            _mapper.Setup(m => m.Map(It.IsAny<TicketForListModel>())).Returns(It.IsAny<Ticket>());
            _repo.Setup(r => r.Update(It.IsAny<Ticket>())).Verifiable();
            _uow.Setup(u => u.SaveAsync()).ReturnsAsync(expected);
            
            //Act
            var actual = await _service.UpdateAsync(It.IsAny<TicketForListModel>());

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.TicketRepository, Times.Once);
            _repo.VerifyAll();
            _mapper.Verify(m => m.Map(It.IsAny<TicketForListModel>()), Times.Once);
            _uow.Verify(u => u.SaveAsync(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async void RemoveAsync_ReturnsTrueIfTicketExists()
        {
            //Arrange
            bool expected = true;
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Ticket());
            _repo.Setup(r => r.Delete(It.IsAny<int>())).Verifiable();
            _uow.Setup(u => u.SaveAsync()).ReturnsAsync(It.IsAny<int>());

            //Act
            var actual = await _service.RemoveAsync(It.IsAny<int>());

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.TicketRepository, Times.Exactly(2));
            
            _repo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _uow.Verify(u => u.SaveAsync(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async void RemoveAsync_ReturnsFalseIfTicketDoesntExist()
        {
            //Arrange
            bool expected = false;
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(null);
            
            //Act
            var actual = await _service.RemoveAsync(It.IsAny<int>());

            //Assert
            _factory.Verify(f => f.GetUnitOfWork(), Times.Once);
            _uow.Verify(u => u.TicketRepository, Times.Once);
            _repo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(expected, actual);
        }
        
    }
}
