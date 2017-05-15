using System.Data.Entity;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Contracts;

namespace WorldOfWords.Infrastructure.Data.EF.Repositories
{
    public class WordSuiteRepository : EfRepository<WordSuite>, IWordSuiteRepository
    {
        public WordSuiteRepository(DbContext dbContext) : base(dbContext) { }

        public void AddOrUpdate(WordSuite entity)
        {
            if (Exists(entity.Id))
            {
                Update(entity);
            }
            else
            {
                Add(entity);
            }
        }
    }
}
