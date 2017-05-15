using System.Collections.Generic;

namespace WorldOfWords.Domain.Models
{
    public class Tag
    {
        public Tag()
        {
            Word = new List<Word>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Word> Word { get; set; } 
    }
}
