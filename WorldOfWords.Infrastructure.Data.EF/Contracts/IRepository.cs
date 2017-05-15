using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorldOfWords.Infrastructure.Data.EF.Contracts
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
        void Add(T entity);
        void Add(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
        void Delete(int id);
    }
}
