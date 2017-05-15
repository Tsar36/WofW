using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorldOfWords.API.Models.Models;

namespace WorldOfWords.API.Models
{
    public class CourseModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public double Progress { get; set;}
        public LanguageModel Language { get; set; }
        public List<CourseWordSuiteModel> WordSuites { get; set; }
    }
}
