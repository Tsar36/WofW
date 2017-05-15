namespace WorldOfWords.API.Models
{
    public class WordProgressModel
    {
        public int WordSuiteId { get; set; }
        public int WordTranslationId { get; set; }
        public bool IsStudentWord { get; set; }
    }
}
