using System.Collections.Generic;

namespace WorldOfWords.API.Models
{
    public class WordTranslationImportModel
    {
        public string OriginalWord { get; set; }
        public string TranslationWord { get; set; }
        public int OriginalWordId { get; set; }
        public int TranslationWordId { get; set; }
        public string Transcription { get; set; }
        public string Description { get; set; }
        public int LanguageId { get; set; }
        public int OwnerId { get; set; }
        public string Comment { get; set; }
        public int? PartOfSpeechId { get; set; }
        public List<TagModel> Tags { get; set; }
    }
}
