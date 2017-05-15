using System.Collections.Generic;

namespace WorldOfWords.Domain.Models
{
    public class Word
    {
        public Word()
        {
            Tags = new List<Tag>();
            Pictures = new List<Picture>();
        }
        public int Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Transcription { get; set; }
        public int LanguageId { get; set; }
        public int? PartOfSpeechId { get; set; } 
        public string Comment { get; set; }
        public virtual Language Language { get; set; }
        public virtual PartsOfSpeech PartOfSpeech { get; set; }
        public virtual ICollection<WordTranslation> WordTranslationsAsWord { get; set; }
        public virtual ICollection<WordTranslation> WordTranslationsAsTranslation { get; set; }
        public virtual ICollection<Tag> Tags { get; set; } 
        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<Record> Records { get; set; }
    }
}
