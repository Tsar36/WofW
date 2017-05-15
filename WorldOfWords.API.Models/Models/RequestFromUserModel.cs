using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfWords.API.Models
{
    public class RequestFromUserModel
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Description { get; set; }
        public int OwnerId { get; set; }
        public int? GroupId { get; set; }
        public bool IsReadByUser { get; set; }
        public bool IsReadByAdmin { get; set; }
    }
}
