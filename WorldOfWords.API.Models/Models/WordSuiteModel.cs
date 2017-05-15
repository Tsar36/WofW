using System.Collections.Generic;

namespace WorldOfWords.API.Models
{
    public class WordSuiteModel
    {
        public string Name { get; set; }
        public int LanguageId { get; set; }
        public int? TranslationLanguageId { get; set; }
        public int Threshold { get; set; }
        public int QuizResponseTime { get; set; }
        public int OwnerId { get; set; }
        public int? PrototypeId { get; set; }
        public List<int> WordTranslationsId { get; set; }
        public ICollection<int> ProhibitedQuizzesId { get; set; }

        public WordSuiteModel()
        {
            ProhibitedQuizzesId = new List<int>();
        }
    }
}
