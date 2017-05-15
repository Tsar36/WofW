namespace WorldOfWords.API.Models
{
    public class WordTranslationModel
    {
        public int Id { get; set; }
        public int Progress { get; set; }
        public bool Result { get; set; }
        public string OriginalWord { get; set; }
        public string OriginalWordDesc { get; set; }
        public string TranslationWord { get; set; }
        public bool IsStudentWord { get; set; }
        public int OriginalWordId { get; set; }
    }
}
