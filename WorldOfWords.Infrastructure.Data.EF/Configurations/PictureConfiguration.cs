using System.Data.Entity.ModelConfiguration;
using WorldOfWords.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    public class PictureConfiguration : EntityTypeConfiguration<Picture>
    {
        public PictureConfiguration()
        {
            //Primary Key
            HasKey(t => t.Id);

            //Properties
            ToTable("Pictures");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.WordId).HasColumnName("WordId").IsRequired();
            Property(t => t.Content).HasColumnName("Content").HasColumnType("varbinary(max)").IsRequired();
        }
    }
}
