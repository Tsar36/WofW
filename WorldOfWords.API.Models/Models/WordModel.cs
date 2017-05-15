using System.Collections.Generic;

namespace WorldOfWords.API.Models
{
    public class WordModel
    {
        public WordModel()
        {
            Tags = new List<TagModel>();
        }
        public int Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Transcription { get; set; }
        public int LanguageId { get; set; }
        public int TranslationLanguageId { get; set; }
        public List<WordValueModel> Synonyms { get; set; }
        public List<WordValueModel> Translations { get; set; }
        public List<TagModel> Tags { get; set; }
        public int OwnerId { get; set; }
        public int? PartOfSpeechId { get; set; }
        public string Comment { get; set; }
    }
}
