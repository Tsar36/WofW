using System.Data.Entity.ModelConfiguration;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    class RecordConfiguration : EntityTypeConfiguration<Record>
    {
        public RecordConfiguration()
        {
            //Primary Key
            HasKey(t => t.Id);

            //Properties
            ToTable("Records");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Content).HasColumnName("Value").IsRequired();
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.WordId).HasColumnName("WordId");
        }
    }
}
