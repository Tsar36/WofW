using System.Collections.Generic;

namespace WorldOfWords.Domain.Models
{
    public class Group
    {
        public Group()
        {
            Enrollments = new List<Enrollment>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }
        public int OwnerId { get; set; }
        public virtual Course Course { get; set; }
        public virtual User Owner { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
