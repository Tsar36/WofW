using System.ComponentModel.DataAnnotations;

namespace WorldOfWords.API.Models
{
    public class LoggedUserModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string EMail { get; set; }
    }   
}
