using System.Data.Entity;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Contracts;

namespace WorldOfWords.Infrastructure.Data.EF.Repositories
{
    public class IncomingUserRepository : EfRepository<IncomingUser>, IIncomingUserRepository
    {
        public IncomingUserRepository(DbContext dbContext) : base(dbContext) { }

        public void AddOrUpdate(IncomingUser entity)
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
