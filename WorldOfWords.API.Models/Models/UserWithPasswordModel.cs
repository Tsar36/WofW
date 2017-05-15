using System.Collections.Generic;

namespace WorldOfWords.API.Models.Models
{
    public class UserWithPasswordModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string HashToken { get; set; }
        public IEnumerable<string> Roles { get; set; }    
    }
}
