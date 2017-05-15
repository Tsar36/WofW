using System;
using System.Data.Entity;
using System.Linq;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Contracts;

namespace WorldOfWords.Infrastructure.Data.EF.Repositories
{
    public class WordProgressRepository : EfRepository<WordProgress>, IWordProgressRepository
    {
        public WordProgressRepository(DbContext dbContext) : base(dbContext) { }

        public new WordProgress GetById(int id)
        {
            throw new InvalidOperationException("Primary key consits of two columns!");
        }

        public new void Delete(int id)
        {
            throw new InvalidOperationException("Primary key consits of two columns!");
        }

        public WordProgress GetById(int suiteId, int translationId)
        {
            return DbSet.FirstOrDefault(wp => wp.WordSuiteId == suiteId && wp.WordTranslationId == translationId);
        }

        public void AddOrUpdate(WordProgress entity)
        {
            if (Exists(entity.WordSuiteId, entity.WordTranslationId))
            {
                Update(entity);
            }
            else
            {
                Add(entity);
            }
        }

        public void Delete(int suiteId, int translationId)
        {
            var entity = GetById(suiteId, translationId);
            if (entity == null)
            {
                return;
            }
            Delete(entity);
        }

        private bool Exists(int suiteId, int translationId)
        {
            return GetById(suiteId, translationId) != null;
        }
    }
}
