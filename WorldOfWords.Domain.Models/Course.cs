using System.Collections.Generic;

namespace WorldOfWords.Domain.Models
{
    public class Course
    {
        public Course()
        {
            WordSuites = new List<WordSuite>();
            Groups = new List<Group>();
        }
     
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public int LanguageId { get; set; }
        //public int NativeLanguageId { get; set; }
        public int OwnerId { get; set; }
        public virtual Language Language { get; set; }
        //public virtual Language NativeLanguage { get; set; }
        public virtual User Owner { get; set; }
        public virtual ICollection<WordSuite> WordSuites { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
    }
}
