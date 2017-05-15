using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class TrainingSynonymTask
    {
        public int WordSuiteId { get; set; }
        public string WordSuiteName { get; set; }
        public int QuizResponseTime { get; set; }
        public List<List<WordValueModel>> Synonyms { get; set; }
        public List<string> Answer { get; set; }
        public List<bool> Result { get; set; }
    }
}
