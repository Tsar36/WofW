using System.ComponentModel.DataAnnotations;

namespace WorldOfWords.API.Models
{
    public class UserForListOfUsersModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public virtual System.Collections.Generic.ICollection<RoleModel> Roles { get; set; }
    }
}
