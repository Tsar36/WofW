using System.Data.Entity.ModelConfiguration;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    public class EnrollmentConfiguration : EntityTypeConfiguration<Enrollment>
    {
        public EnrollmentConfiguration()
        {
            //Primary Key
            HasKey(t => t.Id);

            //Properties
            ToTable("Enrollments");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Date).HasColumnName("Date");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.GroupId).HasColumnName("GroupId");

            //Table & Column Mappings
            HasRequired(t => t.User)
                .WithMany(t => t.Enrollments)
                .HasForeignKey(t => t.UserId);
            HasRequired(t => t.Group)
                .WithMany(t => t.Enrollments)
                .HasForeignKey(t => t.GroupId);
        }
    }
}
