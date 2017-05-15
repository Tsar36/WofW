using System.Data.Entity.ModelConfiguration;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    public class PartsOfSpeechConfiguration : EntityTypeConfiguration<PartsOfSpeech>
    {
        public PartsOfSpeechConfiguration()
        {
            //Primary Key
            HasKey(t => t.Id);

            //Properties
            ToTable("PartsOfSpeech");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ShortName).HasMaxLength(10).HasColumnName("ShortName");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.LanguageId).HasColumnName("LanguageId");

            HasRequired(t => t.Language)
                .WithMany(t => t.PartsOfSpeech)
                .HasForeignKey(t => t.LanguageId);
        }
    }
}
