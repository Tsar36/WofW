using System.Data.Entity.ModelConfiguration;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    public class TagConfiguration: EntityTypeConfiguration<Tag>
    {
        public TagConfiguration()
        {
            //Primary Key
            HasKey(t => t.Id);

            //Properties
            ToTable("Tags");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");

            //Table & Column Mappings
            HasMany(t => t.Word)
                .WithMany(t => t.Tags);
        }
    }
}
