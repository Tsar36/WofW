using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Contracts
{
    public interface IWordSuiteRepository : IRepository<WordSuite>
    {
        void AddOrUpdate(WordSuite entity);
    }
}
