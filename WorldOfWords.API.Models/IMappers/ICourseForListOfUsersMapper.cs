using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface ICourseForListOfUsersMapper
    {
        CourseForListOfUsersModel Map(Course course);
    }
}
