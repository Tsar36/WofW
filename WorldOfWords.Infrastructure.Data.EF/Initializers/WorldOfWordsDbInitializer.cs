using System.Data.Entity;

namespace WorldOfWords.Infrastructure.Data.EF.Initializers
{
    public class WorldOfWordsDbInitializer : DropCreateDatabaseAlways<WorldOfWordsDatabaseContext>
    {
        ////This method allows you to insert data into the database during initialization process
        protected override void Seed(WorldOfWordsDatabaseContext context)
        {
          
        }
    }
}
