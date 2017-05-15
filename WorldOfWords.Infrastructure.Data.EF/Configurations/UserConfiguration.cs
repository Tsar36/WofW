using System.Data.Entity.ModelConfiguration;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            //Primary Key
            HasKey(t => t.Id);

            //Properties
            ToTable("Users");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Password).HasColumnName("Password");
            Property(t => t.Email).HasColumnName("EMail");
            Property(t => t.LanguageId).HasColumnName("LanguageId");

            //Table & Column Mappings
            HasMany(t => t.WordSuites)
                .WithRequired(t => t.Owner);
            HasMany(t => t.Roles)
                .WithMany(t => t.Users)
                .Map(m =>
                {
                    m.ToTable("UserRole");
                    m.MapLeftKey("RoleId");
                    m.MapRightKey("UserId");
                });
            HasMany(t => t.Enrollments)
                .WithRequired(t => t.User);
            HasMany(t => t.Courses)
                .WithRequired(t => t.Owner);
            HasMany(t => t.OwnedGroups)
                .WithRequired(t => t.Owner);
            HasMany(t => t.OwnedWordTranslations)
                .WithRequired(t => t.Owner);
            HasRequired(t => t.Language)
                .WithMany(t => t.Users)
                .HasForeignKey(t => t.LanguageId);
        }
    }
}
