using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface ICourseMapper
    {
        CourseModel Map(Course course);
        List<CourseModel> Map(List<Course> courses);
        Course Map(CourseEditModel course);
    }
}
