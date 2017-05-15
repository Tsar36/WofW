using System.Data.Entity.ModelConfiguration;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    public class CourseConfiguration : EntityTypeConfiguration<Course>
    {
        public CourseConfiguration()
        {
            //Primary Key
            HasKey(t => t.Id);

            //Properties
            ToTable("Roles");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.IsPrivate).HasColumnName("PrivateCourse");
            Property(t => t.LanguageId).HasColumnName("LanguageId");
            //Property(t => t.NativeLanguageId).HasColumnName("NativeLanguageId");
            Property(t => t.OwnerId).HasColumnName("OwnerId");

            //Table & Column Mappings
            HasRequired(t => t.Language)
                .WithMany(t => t.Courses)
                .HasForeignKey(t => t.LanguageId);
            //HasRequired(t => t.NativeLanguage)
            //    .WithMany(t => t.Courses)
            //    .HasForeignKey(t => t.NativeLanguageId);
            HasRequired(t => t.Owner)
                .WithMany(t => t.Courses)
                .HasForeignKey(t => t.OwnerId);
            HasMany(t => t.WordSuites)
                .WithMany(t => t.Courses)
                .Map(m =>
                {
                    m.ToTable("WordSuiteCourse");
                    m.MapRightKey("WordSuiteId");
                    m.MapLeftKey("CourseId");
                });
            HasMany(t => t.Groups)
                .WithRequired(t => t.Course);
        }
    }
}
