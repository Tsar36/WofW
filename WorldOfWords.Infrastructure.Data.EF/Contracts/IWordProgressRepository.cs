using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Contracts
{
    public interface IWordProgressRepository : IRepository<WordProgress>
    {
        WordProgress GetById(int suiteId, int translationId);
        void AddOrUpdate(WordProgress entity);
        void Delete(int suiteId, int translationId);
    }
}
