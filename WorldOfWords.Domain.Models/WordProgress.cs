using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldOfWords.Domain.Models
{
    public class WordProgress
    {
        [Key, Column(Order = 0)]
        public int WordSuiteId { get; set; }
        [Key, Column(Order = 1)]
        public int WordTranslationId { get; set; }
        public int? Progress { get; set; }
        public bool IsStudentWord { get; set; }
        public virtual WordSuite WordSuite { get; set; }
        public virtual WordTranslation WordTranslation { get; set; }
    }
}
