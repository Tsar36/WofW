using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using System.Data.Entity.Migrations;

namespace WorldOfWords.Infrastructure.Data.EF.Repositories
{
    public class WordTranslationRepository : EfRepository<WordTranslation>, IWordTranslationRepository
    {
        public WordTranslationRepository(DbContext dbContext) : base(dbContext) { }

        public new WordTranslation GetById(int id)
        {
            throw new InvalidOperationException("Primary key consits of two columns!");
        }

        public new void Delete(int id)
        {
            throw new InvalidOperationException("Primary key consits of two columns!");
        }

        public WordTranslation GetById(int originalWordId, int translationWordId)
        {
            return DbSet.FirstOrDefault(wp => wp.OriginalWordId == originalWordId && wp.TranslationWordId == translationWordId);
        }
        public async Task<WordTranslation> GetByIdAsync(int originalWordId, int translationWordId)
        {
            return await DbSet.FirstOrDefaultAsync(wp => wp.OriginalWordId == originalWordId && wp.TranslationWordId == translationWordId);
        }

        public void AddOrUpdate(WordTranslation entity)
        {
            if (Exists(entity.OriginalWordId, entity.TranslationWordId))
            {
                Update(entity);
            }
            else
            {
                Add(entity);
            }
        }

        public void Update(WordTranslation entity)
        {
            DbSet.AddOrUpdate(e => new { e.OriginalWordId, e.TranslationWordId }, entity);
        }

        public void Delete(int originalWordId, int translationWordId)
        {
            var entity = GetById(originalWordId, translationWordId);
            if (entity == null)
            {
                return;
            }
            Delete(entity);
        }

        private bool Exists(int originalWordId, int translationWordId)
        {
            return GetById(originalWordId, translationWordId) != null;
        }
    }
}
