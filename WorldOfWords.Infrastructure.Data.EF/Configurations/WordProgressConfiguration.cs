using System.Data.Entity.ModelConfiguration;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    public class WordProgressConfiguration: EntityTypeConfiguration<WordProgress>
    {
        public WordProgressConfiguration()
        {
            //Primary Key
            HasKey(t => new { t.WordSuiteId, t.WordTranslationId });

            //Properties
            ToTable("WordProgresses");
            Property(t => t.WordSuiteId).HasColumnName("WordSuiteId");
            Property(t => t.WordTranslationId).HasColumnName("WordTranslationId");
            Property(t => t.Progress).HasColumnName("Progress");
            Property(t => t.IsStudentWord).HasColumnName("IsStudentWord");

            //Table & Column Mappings
            HasRequired(t => t.WordSuite)
                .WithMany(t => t.WordProgresses)
                .HasForeignKey(t => t.WordSuiteId);
            HasRequired(t => t.WordTranslation)
                .WithMany(t => t.WordProgresses)
                .HasForeignKey(t => t.WordTranslationId);
        }
    }
}
