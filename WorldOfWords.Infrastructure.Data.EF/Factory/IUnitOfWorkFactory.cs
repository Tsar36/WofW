using WorldOfWords.Infrastructure.Data.EF.UnitOfWork;

namespace WorldOfWords.Infrastructure.Data.EF.Factory
{
    public interface IUnitOfWorkFactory
    {
        IWorldOfWordsUow GetUnitOfWork();
    }
}
