using System.Collections.Generic;

namespace WorldOfWords.Domain.Models
{
    public class WordTranslation
    {
        public WordTranslation()
        {
            WordProgresses = new List<WordProgress>();
        }

        public int Id { get; set; }      
        public int OriginalWordId { get; set; }
        public int TranslationWordId { get; set; }
        public int OwnerId { get; set; }

        public virtual Word OriginalWord { get; set; }
        public virtual Word TranslationWord { get; set; }
        public virtual User Owner { get; set; }
        public virtual ICollection<WordProgress> WordProgresses { get; set; }
    }
}
