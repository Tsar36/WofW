namespace WorldOfWords.API.Models
{
    public class EnrollmentModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public int GroupId { get; set; }
        public UserForListingModel User { get; set; }
    }
}
