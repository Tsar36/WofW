using System.Collections.Generic;

namespace WorldOfWords.API.Models
{
    public class WordTranslationFullModel
    {
        public WordTranslationFullModel()
        {
            Translations = new List<WordValueModel>();
            Synonims = new List<WordValueModel>();
            Tags = new List<TagModel>();
        }
        public int Id { get; set; }
        public string OriginalWord { get; set; }
        public string Transcription { get; set; }
        public string Description { get; set; }
        public int? PartOfSpeechId { get; set; }
        public string Comment { get; set; }
        public List<WordValueModel> Translations { get; set; }
        public List<WordValueModel> Synonims { get; set; }
        public List<TagModel> Tags { get; set; }
    }
}
