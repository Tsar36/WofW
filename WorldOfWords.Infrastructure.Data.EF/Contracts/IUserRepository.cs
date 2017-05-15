using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
        void AddOrUpdate(User entity);
    }
}
