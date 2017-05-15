using System.Collections.Generic;

namespace WorldOfWords.API.Models
{
    public class WordTranslationEditModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Transcription { get; set; }
        public int OriginalLanguageId { get; set; }
        public int TranslationLanguageId { get; set; }
        public int OwnerId { get; set; }
        public int? PartOfSpeechId { get; set; }
        public string Comment { get; set; }

        public List<WordValueModel> TranslationsToAddRange { get; set; }
        public List<int> TranslationsToDeleteIdRange { get; set; }
        public List<WordValueModel> SynonymsToAddIdRange { get; set; }
        public List<int> SynonymsToDeleteIdRange { get; set; }
        public List<TagModel> TagsToAddRange { get; set; }
        public List<int> TagsToDeleteIdRange { get; set; }
    }
}
