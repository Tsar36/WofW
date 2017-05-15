using System.Data.Entity;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Contracts;

namespace WorldOfWords.Infrastructure.Data.EF.Repositories
{
    public class UserRepository : EfRepository<User>, IUserRepository
    {
        public UserRepository(DbContext dbContext) : base(dbContext) { }

        public void AddOrUpdate(User entity)
        {
            if(Exists(entity.Id))
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
