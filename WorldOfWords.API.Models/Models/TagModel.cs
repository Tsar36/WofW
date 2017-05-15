using System.ComponentModel.DataAnnotations;

namespace WorldOfWords.API.Models
{
    public class TagModel
    {
        public int? Id { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
