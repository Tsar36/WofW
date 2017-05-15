using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Services;
using WorldOfWords.Domain.Services.MessagesAndConsts;

namespace WorldofWords.Controllers
{
    [WowAuthorization(AllRoles = new[] { "Student", "Teacher" })]
    [RoutePrefix("api/course")]
    public class CourseController : BaseController
    {
        private readonly ICourseMapper _courseMapper;
        private readonly ICourseService _courseService;
        private readonly IWordSuiteService _wordSuiteService;

        public CourseController(ICourseMapper courseMapper, ICourseService courseService,
            IWordSuiteService wordSuiteService)
        {
            _courseMapper = courseMapper;
            _courseService = courseService;
            _wordSuiteService = wordSuiteService;
        }

        [Route("StudentCourses")]
        public List<CourseModel> GetStudentCourses()
        {
            return _courseMapper.Map(_courseService.GetStudentCourses(UserId));
        }

        [Route("TeacherCourses")]
        public List<CourseModel> GetTeacherCourses()
        {
            return _courseMapper.Map(_courseService.GetTeacherCourses(UserId));
        }

        [Route("Progress")]
        public IHttpActionResult GetCourseProgressByUserId(int courseId, int userId)
        {
            var course = _courseService.GetById(courseId, userId);
            if (course != null)
            {
                var courseModel = _courseMapper.Map(course);
                courseModel.Progress = _courseService.GetProgress(courseId, userId);
                foreach (var wordSuite in courseModel.WordSuites)
                {
                    wordSuite.Progress = _wordSuiteService.GetWordSuiteProgress(wordSuite.Id);
                }
                return Ok(courseModel);
            }
            return BadRequest(MessagesContainer.IncorrectData);
        }

        public IHttpActionResult Delete(int courseId)
        {
            var course = _courseService.GetById(courseId);
            if (course != null)
            {
                if (course.Groups.Count != 0)
                {
                    return BadRequest(MessagesContainer.DeleteGroupFirst);
                }
                _courseService.Delete(courseId);
                return Ok();
            }
            return BadRequest(MessagesContainer.IncorrectData);
        }

        [Route("CreateCourse")]
        public IHttpActionResult CreateCourse(CourseEditModel courseModel)
        {
            if (courseModel != null)
            {
                var course = _courseMapper.Map(courseModel);
                List<int> wordSuitesId = courseModel.WordSuites.Select(x => x.Id).ToList();
                course.Id = _courseService.Add(course, wordSuitesId);
                return Ok();
            }
            return BadRequest(MessagesContainer.IncorrectData);
        }

        [Route("EditCourse")]
        public IHttpActionResult EditCourse(CourseEditModel courseModel)
        {
            if (courseModel != null)
            {
                var course = _courseMapper.Map(courseModel);
                var wordSuitesId = courseModel.WordSuites.Select(x => x.Id).ToList();
                _courseService.EditCourse(course, wordSuitesId);
                return Ok();
            }
            return BadRequest(MessagesContainer.IncorrectData);
        }

        public IHttpActionResult GetById(int id)
        {
            var course = _courseService.GetById(id);
            if (course != null)
            {
                return Ok(_courseMapper.Map(course));
            }
            return BadRequest(MessagesContainer.IncorrectData);
        }

    }
}