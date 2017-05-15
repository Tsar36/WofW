using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfWords.Domain.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public int OwnerId { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public int ReviewStatus { get; set; }      //Active, InProgress, Rejected, Done
        // For ability to make request to add into group
        public int? GroupId { get; set; }
        public bool IsReadByUser { get; set; }
        public bool IsReadByAdmin { get; set; }

        public virtual User User { get; set; }
        public virtual Group Group { get; set; }
    }
}
