using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Configurations;
using WorldOfWords.Infrastructure.Data.EF.Migrations;

namespace WorldOfWords.Infrastructure.Data.EF
{
    public class WorldOfWordsDatabaseContext : DbContext
    {
        static WorldOfWordsDatabaseContext()
        {
            //Disables DbInitializers
            //Database.SetInitializer<WorldOfWordsDatabaseContext>(null);

            //Examples of DbInitializers:
            //Names mentions what they are doing
            //Database.SetInitializer<WorldOfWordsDatabaseContext>(new CreateDatabaseIfNotExists<WorldOfWordsDatabaseContext>());
            //Database.SetInitializer<WorldOfWordsDatabaseContext>(new DropCreateDatabaseAlways<WorldOfWordsDatabaseContext>());
            //Database.SetInitializer<WorldOfWordsDatabaseContext>(new DropCreateDatabaseIfModelChanges<WorldOfWordsDatabaseContext>());
            
            //Use Custom DbInitializer
            //Database.SetInitializer<WorldOfWordsDatabaseContext>(new WorldOfWordsDbInitializer());

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<WorldOfWordsDatabaseContext, Configuration>("WorldOfWordsDatabaseContext"));
            //Database.SetInitializer<WorldOfWordsDatabaseContext>(null);
        }

        public WorldOfWordsDatabaseContext()
            : base("Name=WorldOfWordsDatabaseContext")
        {
        }

        public DbSet<IncomingUser> IncomingUsers { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<WordSuite> WordSuites { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<WordProgress> WordProgresses { get; set; }
        public DbSet<WordTranslation> WordTranslations { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<PartsOfSpeech> PartsOfSpeech { get; set; } 
        public DbSet<Quiz> Quizzes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Configurations.Add(new LanguageConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new WordConfiguration());
            modelBuilder.Configurations.Add(new WordSuiteConfiguration());
            modelBuilder.Configurations.Add(new WordTranslationConguration());
            modelBuilder.Configurations.Add(new GroupConfiguration());
            modelBuilder.Configurations.Add(new EnrollmentConfiguration());
            modelBuilder.Configurations.Add(new TagConfiguration());
            modelBuilder.Configurations.Add(new TicketConfiguration());
            modelBuilder.Configurations.Add(new PictureConfiguration());
            modelBuilder.Configurations.Add(new RecordConfiguration());
            modelBuilder.Configurations.Add(new PartsOfSpeechConfiguration()); 
            modelBuilder.Configurations.Add(new QuizConfiguration());
        }
    }
}
