using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfWords.API.Models.Models
{
    public class ForgotPasswordUserModel
    {
        [Required]
        public string Password { get; set; }

        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
        public string PagesUrl { get; set; }
    }
}
