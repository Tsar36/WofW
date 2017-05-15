using System.ComponentModel.DataAnnotations;

namespace WorldOfWords.API.Models
{
    public class LoginUserModel
    {
        public LoginUserModel(int id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
        public LoginUserModel() {}
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
