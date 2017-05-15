using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfWords.API.Models
{
    public class QuizWordSuiteModel
    {
        public int Id { get; set; }
        public int QuizResponseTime { get; set; }
        public DateTime? QuizStartTime { get; set; }
        public string Name { get; set; }
        public List<WordTranslationModel> WordTranslations { get; set; }
    }
}
