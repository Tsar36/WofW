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
using WorldOfWords.Domain.Services.Services;
using WorldOfWords.API.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace WorldOfWords.Tests.ServicesTests
{
    [TestFixture]
    class RecordServiceTest
    {
        private Mock<IUnitOfWorkFactory> _factory;
        private Mock<IWorldOfWordsUow> _uow;
        private Mock<IRepository<Record>> _repo;
        private RecordService _service;
        private Mock<IRecordMapper> _mapper;

        [SetUp]
        public void Setup()
        {
            _factory = new Mock<IUnitOfWorkFactory>();
            _uow = new Mock<IWorldOfWordsUow>();
            _repo = new Mock<IRepository<Record>>();
            _mapper = new Mock<IRecordMapper>();

            _service = new RecordService(_factory.Object, _mapper.Object);

            _factory.Setup(f => f.GetUnitOfWork()).Returns(_uow.Object);
            _uow.Setup(u => u.RecordRepository).Returns(_repo.Object);
        }



    }
}
