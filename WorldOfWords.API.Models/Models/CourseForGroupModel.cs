using System.ComponentModel.DataAnnotations;

namespace WorldOfWords.API.Models
{
    public class CourseForGroupModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
