using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfWords.API.Models
{
    public class MixTaskQuestionModel
    {
        public List<WordValueModel> Synonyms { get; set; }
        public WordValueModel Description { get; set; }
        // id is translation Id
        // value is translationWord.value
        public WordValueModel Translation { get; set; }
    }
}
