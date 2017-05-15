using System.Collections.Generic;

namespace WorldOfWords.Domain.Models
{
    public partial class Language
    {
        public Language()
        {
            Words = new List<Word>();
            WordSuites = new List<WordSuite>();
            TranslationLanguagesForWordSuites = new List<WordSuite>();
            Courses = new List<Course>();
            Users = new List<User>();
            PartsOfSpeech = new List<PartsOfSpeech>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public virtual ICollection<Word> Words { get; set; }
        public virtual ICollection<WordSuite> WordSuites { get; set; }
        public virtual ICollection<WordSuite> TranslationLanguagesForWordSuites { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<PartsOfSpeech> PartsOfSpeech { get; set; } 
    }
}
