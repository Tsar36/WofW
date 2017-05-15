using System.Data.Entity.ModelConfiguration;
using WorldOfWords.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    public class QuizConfiguration : EntityTypeConfiguration<Quiz>
    {
        public QuizConfiguration()
        {
            HasKey(t => t.Id);

            ToTable("Quizzes");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");

            HasMany(t => t.ProhibitedWordSuits)
               .WithMany(q => q.ProhibitedQuizzes)
               .Map(m =>
               {
                   m.ToTable("WordSuiteQuiz");
                   m.MapRightKey("WordSuiteId");
                   m.MapLeftKey("QuizId");
               });
        }
    }
}
