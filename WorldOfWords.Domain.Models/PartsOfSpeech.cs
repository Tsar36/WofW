using System.Collections.Generic;

namespace WorldOfWords.Domain.Models
{
    public class PartsOfSpeech
    {
        public PartsOfSpeech()
        {
            Words = new List<Word>();
        }

        public int Id { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public int LanguageId { get; set; }

        public virtual Language Language { get; set; }
        public virtual ICollection<Word> Words { get; set; }
    }
}