using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorldOfWords.API.Models
{
    public class SearchWordTranslationModel
    {
        [Required]
        public string SearchWord {get; set;}
        public List<string> Tags { get; set; }
        public int LanguageId { get; set; }
        public bool SearchByTag { get; set; }
    }
}
