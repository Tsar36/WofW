using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfWords.API.Models.Models
{
    public class TicketForListModel
    {
        public int TicketId { get; set; }
        [Required]
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public TicketReviewStatus ReviewStatus { get; set; }
        [Required]
        public int OwnerId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int? GroupId { get; set; }
        public string ReviewStatusString
        {
            get { return ReviewStatus.ToString(); }
        }
        public bool IsReadByUser { get; set; }
        public bool IsReadByAdmin { get; set; }
    }
}
