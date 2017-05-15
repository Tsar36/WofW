using System.Data.Entity.ModelConfiguration;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    public class LanguageConfiguration : EntityTypeConfiguration<Language>
    {
        public LanguageConfiguration()
        {
            //Primary Key
            HasKey(t => t.Id);

            //Properties
            ToTable("Languages");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");

            //Table & Column Mappings
            HasMany(t => t.Words)
                .WithRequired(t => t.Language);
            HasMany(t => t.WordSuites)
                .WithRequired(t => t.Language);
            HasMany(t => t.Courses)
                .WithRequired(t => t.Language);
            HasMany(t => t.Users)
                .WithRequired(t => t.Language);
            HasMany(t => t.PartsOfSpeech)
                .WithRequired(t => t.Language);
        }
    }
}
