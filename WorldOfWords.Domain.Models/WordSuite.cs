using System;
using System.Collections.Generic;

namespace WorldOfWords.Domain.Models
{
    public class WordSuite
    {
        public WordSuite()
        {
            WordProgresses = new List<WordProgress>();
            Courses = new List<Course>();
            DerivedWordSuites = new List<WordSuite>();
            ProhibitedQuizzes = new List<Quiz>();
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public int LanguageId { get; set; }
        public int? TranslationLanguageId { get; set; }
        public int Threshold { get; set; }
        public int QuizResponseTime { get; set; }
        public DateTime? QuizStartTime { get; set; }
        public int OwnerId { get; set; }
        public Nullable<int> PrototypeId { get; set; }

        public virtual User Owner { get; set; }
        public virtual WordSuite PrototypeWordSuite { get; set; }
        public virtual Language Language { get; set; }
        public virtual Language TranslationLanguage { get; set; }
        public virtual ICollection<WordSuite> DerivedWordSuites { get; set; }
        public virtual ICollection<Course> Courses { get; set; } 
        public virtual ICollection<WordProgress> WordProgresses { get; set; }
        public virtual ICollection<Quiz> ProhibitedQuizzes { get; set; }
    }
}
