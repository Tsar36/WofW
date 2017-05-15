using System.Collections.Generic;

namespace WorldOfWords.Domain.Models
{
    public class Record
    {
        public int Id { get; set; }
        public byte[] Content { get; set; }
        public string Description { get; set; }
        public int WordId { get; set; }
        public virtual Word Word { get; set; }
    }
}
