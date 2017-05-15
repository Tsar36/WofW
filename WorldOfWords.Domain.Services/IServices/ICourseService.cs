using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Domain.Services
{
    public interface ICourseService
    {
        List<Course> GetAllCourses(int userId);
        List<Course> GetStudentCourses(int userId);
        List<Course> GetTeacherCourses(int userId);
        Course GetById(int id, int userId);
        Course GetById(int id);
        void Delete(int courseId);
        double GetProgress(int id, int userId);
        int Add(Course course, List<int> wordSuitesId);
        void EditCourse(Course course, List<int> wordSuitesId);
        IList<string> GetUsersIdByCourseId(int courseId);
    }
}