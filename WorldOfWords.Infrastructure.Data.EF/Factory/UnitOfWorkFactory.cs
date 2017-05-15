using WorldOfWords.Infrastructure.Data.EF.UnitOfWork;

namespace WorldOfWords.Infrastructure.Data.EF.Factory
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IWorldOfWordsUow GetUnitOfWork()
        {
            return new WorldOfWordsUow();
        }
    }
}
