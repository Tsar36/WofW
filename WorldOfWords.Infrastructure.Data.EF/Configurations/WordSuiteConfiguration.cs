using System.Data.Entity.ModelConfiguration;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    public class WordSuiteConfiguration : EntityTypeConfiguration<WordSuite>
    {
        public WordSuiteConfiguration()
        {
            //Primary Key
            HasKey(t => t.Id);

            //Properties
            ToTable("WordSuites");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.LanguageId).HasColumnName("LanguageId");
            Property(t => t.TranslationLanguageId).HasColumnName("TranslationLanguageId").IsOptional();
            Property(t => t.Threshold).HasColumnName("Threshold");
            Property(t => t.QuizResponseTime).HasColumnName("QuizResponseTime");
            Property(t => t.QuizStartTime).HasColumnName("QuizStartTime");
            Property(t => t.OwnerId).HasColumnName("OwnerId");
            Property(t => t.PrototypeId).HasColumnName("PrototypeId").IsOptional();


            HasRequired(t => t.Owner)
                .WithMany(t => t.WordSuites)
                .HasForeignKey(t => t.OwnerId);
            HasOptional(t => t.PrototypeWordSuite)
                .WithMany(t => t.DerivedWordSuites)
                .HasForeignKey(t=>t.PrototypeId);
            HasRequired(t => t.Language)
                .WithMany(t => t.WordSuites)
                .HasForeignKey(t => t.LanguageId);
            HasOptional(t => t.TranslationLanguage)
                .WithMany(t => t.TranslationLanguagesForWordSuites)
                .HasForeignKey(t => t.TranslationLanguageId);
            HasMany(t => t.DerivedWordSuites)
                .WithOptional(t => t.PrototypeWordSuite);
            HasMany(t => t.Courses)
                .WithMany(t => t.WordSuites)
                .Map(m =>
                {
                    m.ToTable("WordSuiteCourse");
                    m.MapRightKey("CourseId");
                    m.MapLeftKey("WordSuiteId");
                });
            HasMany(t => t.WordProgresses)
                .WithRequired(t => t.WordSuite);

            HasMany(t => t.ProhibitedQuizzes)
                .WithMany(q => q.ProhibitedWordSuits)
                .Map(m =>
                {
                    m.ToTable("WordSuiteQuiz");
                    m.MapRightKey("QuizId");
                    m.MapLeftKey("WordSuiteId");
                });
        }
    }
}
