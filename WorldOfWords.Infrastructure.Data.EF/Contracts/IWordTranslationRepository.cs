using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Contracts
{
    public interface IWordTranslationRepository : IRepository<WordTranslation>
    {
        WordTranslation GetById(int suiteId, int translationId);
        Task<WordTranslation> GetByIdAsync(int suiteId, int translationId);
        void AddOrUpdate(WordTranslation entity);
        void Delete(int suiteId, int translationId);
    }
}
