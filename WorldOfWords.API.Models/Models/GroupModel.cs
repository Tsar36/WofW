using System.ComponentModel.DataAnnotations;

namespace WorldOfWords.API.Models
{
    public class GroupModel
    {
        [Required]
        public string Name { get; set; }
        public int CourseId { get; set; }
        public int OwnerId { get; set; }
    }
}
