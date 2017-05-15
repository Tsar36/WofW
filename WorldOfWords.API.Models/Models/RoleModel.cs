using System.ComponentModel.DataAnnotations;

namespace WorldOfWords.API.Models
{
    public class RoleModel
    {
        public int Id;
        [Required]
        public string Name;
    }
}
