using System.Collections.Generic;

namespace WorldOfWords.Domain.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new List<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
