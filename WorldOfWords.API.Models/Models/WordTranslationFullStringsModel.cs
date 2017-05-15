using System.Collections.Generic;

namespace WorldOfWords.API.Models
{
    public class WordTranslationFullStringsModel
    {
        public string OriginalWord { get; set; }
        public string Transcription { get; set; }
        public string Description { get; set; }
        public string PartOfSpeech { get; set; }
        public string Comment { get; set; }
        public List<string> Translations { get; set; }
        public List<string> Synonims { get; set; }
        public List<string> Tags { get; set; }

    }
}
