using System.Collections.Generic;
using System;

namespace WorldOfWords.API.Models
{
    public class TrainingWordSuiteModel
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int QuizResponseTime { get; set; }
        public int Threshold { get; set; }
        public int LanguageId { get; set; }
        public DateTime? QuizStartTime { get; set; }
        public string Name { get; set; }
        public List<WordTranslationModel> WordTranslations { get; set; }
    }
}
