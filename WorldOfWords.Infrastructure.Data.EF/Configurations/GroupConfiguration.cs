using System.Data.Entity.ModelConfiguration;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    public class GroupConfiguration : EntityTypeConfiguration<Group>
    {
        public GroupConfiguration()
        {
            //Primary Key
            HasKey(t => t.Id);

            //Properties
            ToTable("Groups");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.CourseId).HasColumnName("CourseId");
            Property(t => t.OwnerId).HasColumnName("OwnerId");

            //Table & Column Mappings
            HasRequired(t => t.Course)
                .WithMany(t => t.Groups)
                .HasForeignKey(t => t.CourseId);
            HasRequired(t => t.Owner)
                .WithMany(t => t.OwnedGroups)
                .HasForeignKey(t => t.OwnerId);
            HasMany(t => t.Enrollments)
                .WithRequired(t => t.Group);
        }
    }
}
