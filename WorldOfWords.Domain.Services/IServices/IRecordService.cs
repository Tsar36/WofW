using System.Collections.Generic;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;
using WorldOfWords.API.Models;

namespace WorldOfWords.Domain.Services
{
    public interface IRecordService
    {
        Task<RecordModel> GetRecordModelByIdAsync(int recordId);
        Task<RecordModel> GetRecordModelByWordIdAsync(int wordId);
        Task<bool> IsThereRecord(int wordId);
        Task<bool> AddAsync(RecordModel record);
        Task<bool> DeleteRecordAsync(int wordId);
    }
}
