using System.Collections.Generic;

namespace WorldOfWords.API.Models
{
    public class CourseWordSuiteModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Progress { get; set; }
        public ICollection<int> ProhibitedQuizzesId { get; set; }

        public CourseWordSuiteModel()
        {
            ProhibitedQuizzesId = new List<int>();
        }
    }
}
