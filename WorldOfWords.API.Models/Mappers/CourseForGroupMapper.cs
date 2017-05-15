using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class CourseForGroupMapper : ICourseForGroupMapper
    {
        public CourseForGroupModel Map(Course course)
        {
            return new CourseForGroupModel() { Id = course.Id, Name = course.Name };
        }
        public List<CourseForGroupModel> Map(IEnumerable<Course> courses)
        {
            List<CourseForGroupModel> courseModels = new List<CourseForGroupModel>();
            foreach(var course in courses)
            {
                courseModels.Add(Map(course));
            }
            return courseModels;
        }
    }
}
