using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface ICourseForGroupMapper
    {
        CourseForGroupModel Map(Course course);
        List<CourseForGroupModel> Map(IEnumerable<Course> courses);
    }
}
