using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Contracts
{
    public interface IIncomingUserRepository : IRepository<IncomingUser>
    {
        void AddOrUpdate(IncomingUser entity);
    }
}
