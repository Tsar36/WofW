namespace WorldOfWords.API.Models
{
    public class GroupForListingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
    }
}
