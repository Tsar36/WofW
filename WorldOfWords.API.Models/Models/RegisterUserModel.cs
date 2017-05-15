using System.ComponentModel.DataAnnotations;

namespace WorldOfWords.API.Models
{
    public class RegisterUserModel
    {
        public int Id { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int LanguageId { get; set; }
        public string PagesUrl { get; set; }
    }
}
