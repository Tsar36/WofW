using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class CourseForListOfUsersMapper: ICourseForListOfUsersMapper
    {
        public CourseForListOfUsersModel Map(Course course)
        {
            return new CourseForListOfUsersModel()
            {
                Id = course.Id,
                Name = course.Name
            };
        }
    }
}
