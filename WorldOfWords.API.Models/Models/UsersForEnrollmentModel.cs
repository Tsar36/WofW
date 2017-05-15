using System.Collections.Generic;

namespace WorldOfWords.API.Models
{
    public class UsersForEnrollmentModel
    {
        public List<UserForListingModel> UserModels { get; set; }
        public int GroupId { get; set; }
    }
}
