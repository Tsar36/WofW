using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfWords.Domain.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public byte[] Content { get; set; }
        public int WordId { get; set; }
        public virtual Word Word { get; set; }
    }
}
