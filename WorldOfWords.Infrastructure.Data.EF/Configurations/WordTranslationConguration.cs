using System.Data.Entity.ModelConfiguration;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    public class WordTranslationConguration: EntityTypeConfiguration<WordTranslation>
    {
        public WordTranslationConguration()
        {
            //Primary Key
            HasKey(t => t.Id);

            //Properties
            ToTable("WordTranslations");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OriginalWordId).HasColumnName("OriginalWordId");
            Property(t => t.TranslationWordId).HasColumnName("TranslationWordId");
            Property(t => t.OwnerId).HasColumnName("OwnerId");

            //Table & Column Mappings
            HasRequired(t => t.OriginalWord)
                .WithMany(t => t.WordTranslationsAsWord)
                .HasForeignKey(t => t.OriginalWordId);
            HasRequired(t => t.TranslationWord)
                .WithMany(t => t.WordTranslationsAsTranslation)
                .HasForeignKey(t => t.TranslationWordId);
            HasRequired(t => t.Owner)
                .WithMany(t => t.OwnedWordTranslations)
                .HasForeignKey(t => t.OwnerId);
            HasMany(t => t.WordProgresses)
                .WithRequired(t => t.WordTranslation);
        }
    }
}
