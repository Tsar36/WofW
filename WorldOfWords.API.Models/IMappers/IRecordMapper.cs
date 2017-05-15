using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IRecordMapper
    {
        Record ToDomainModel(RecordModel apiModel);
        RecordModel ToApiModel(Record domainModel);
    }
}
