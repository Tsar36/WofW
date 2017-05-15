using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfWords.Domain.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<WordSuite> ProhibitedWordSuits { get; set; }
        public Quiz()
        {
            this.ProhibitedWordSuits = new List<WordSuite>();
        }
    }
}
