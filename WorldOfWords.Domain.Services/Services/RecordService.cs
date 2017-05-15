using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using System;
using System.Text;
using WorldOfWords.Domain.Services.IServices;
using System.Data.Entity;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Models;
using WorldOfWords.API.Models;

namespace WorldOfWords.Domain.Services.Services
{
    public class RecordService : IRecordService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IRecordMapper _recordMapper;

        public RecordService(IUnitOfWorkFactory unitOfWorkFactory, IRecordMapper recordMapper)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _recordMapper = recordMapper;
        }

        public async Task<RecordModel> GetRecordModelByIdAsync(int recordId)
        {
            using(var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var record = await uow.RecordRepository.GetByIdAsync(recordId);
                return record != null ? _recordMapper.ToApiModel(record) : null;
            }
        }

        public async Task<RecordModel> GetRecordModelByWordIdAsync(int wordId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var record = await uow.RecordRepository.GetAll().FirstOrDefaultAsync(r => r.WordId == wordId);
                return record != null ? _recordMapper.ToApiModel(record) : null;
            }
        }

        public async Task<bool> IsThereRecord(int wordId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var record = await uow.RecordRepository.GetAll().FirstOrDefaultAsync(r => r.WordId == wordId);
                return record != null;
            }
        }

        public async Task<bool> AddAsync(RecordModel record)
        {
            using(var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var recordToAdd = _recordMapper.ToDomainModel(record);
                uow.RecordRepository.Add(recordToAdd);
                return await uow.SaveAsyncBool();
            }
        }

        public async Task<bool> DeleteRecordAsync(int wordId)
        {
            using(var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var entity = await uow.RecordRepository.GetAll().FirstOrDefaultAsync(r => r.WordId == wordId);
                uow.RecordRepository.Delete(entity);
                return await uow.SaveAsync() > 0;
            }
        }

    }
}
