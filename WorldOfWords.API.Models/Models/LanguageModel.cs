using System.ComponentModel.DataAnnotations;

namespace WorldOfWords.API.Models.Models
{
    public class LanguageModel
    {
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ShortName { get; set; }
    }
}
