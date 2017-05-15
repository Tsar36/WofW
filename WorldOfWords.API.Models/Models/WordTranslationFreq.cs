using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfWords.API.Models
{
    public class WordTranslationFreq
    {
        public int Id { get; set; }
        public string OriginalWord { get; set; }
        public string TranslationWord { get; set; }
        public int Freq { get; set; }
    }
}
