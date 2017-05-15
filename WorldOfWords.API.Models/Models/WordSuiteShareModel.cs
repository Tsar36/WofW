using System.Collections.Generic;

namespace WorldOfWords.API.Models
{
    //mfomitc
    public class WordSuiteShareModel
    {
        public string Name { get; set; }
        public int LanguageId { get; set; }
        public int? TranslationLanguageId { get; set; }
        public int Threshold { get; set; }
        public bool IsPictureQuizAllowed { get; set; }
        public bool IsSoundQuizAllowed { get; set; }
        public int QuizResponseTime { get; set; }
        public int? OwnerId { get; set; }
        public int? PrototypeId { get; set; }
    }
}
