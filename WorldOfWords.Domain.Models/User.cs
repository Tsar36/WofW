using System.Collections.Generic;

namespace WorldOfWords.Domain.Models
{
    public partial class User
    {
        public User()
        {
            WordSuites=new List<WordSuite>();
            Roles = new List<Role>();
            Enrollments = new List<Enrollment>();
            Courses=new List<Course>();
            OwnedGroups=new List<Group>();
            OwnedWordTranslations=new List<WordTranslation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string HashedToken { get; set; }
        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }
        public virtual ICollection<WordSuite> WordSuites { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<Group> OwnedGroups { get; set; }
        public virtual ICollection<WordTranslation> OwnedWordTranslations { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
