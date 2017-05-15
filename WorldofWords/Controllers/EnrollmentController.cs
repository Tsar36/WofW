using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services;

namespace WorldofWords.Controllers
{
    [WowAuthorization(Roles = "Teacher")]
    [RoutePrefix("api/Enrollment")]
    public class EnrollmentController : BaseController
    {
        private readonly IEnrollmentMapper _enrollmentMapper;
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IWordSuiteService _wordSuiteService;
        private readonly IWordProgressService _wordProgressService;
        private readonly IGroupService _groupService;
        private readonly IUserForListingMapper _userMapper;

        public EnrollmentController(IEnrollmentService enrollmentService, IEnrollmentMapper enrollmentMapper,
            IWordSuiteService wordSuiteService, IWordProgressService wordProgressService, IUserForListingMapper userMapper,
            ICourseService courseService, IGroupService groupService)
        {
            _enrollmentService = enrollmentService;
            _enrollmentMapper = enrollmentMapper;
            _wordSuiteService = wordSuiteService;
            _wordProgressService = wordProgressService;
            _userMapper = userMapper;
            _courseService = courseService;
            _groupService = groupService;
        }

        [Route("getEnrollmentsByGroupId")]
        public List<EnrollmentWithProgressModel> GetByGroupId(int groupId)
        {
            List<EnrollmentModel> enrollments = _enrollmentMapper.Map(_enrollmentService.GetByGroupId(groupId));
            Group currGroup = _groupService.GetById(groupId, UserId);
            if(currGroup == null)
            {
                return null;
            }
            return enrollments.Select(e => new EnrollmentWithProgressModel
                {
                    Enrollment = e,
                    Progress = _courseService.GetProgress(currGroup.CourseId, e.User.Id)
                }).ToList();
        }

        [Route("getUsersNotBelongingToGroup")]
        public List<UserForListingModel> GetUsersNotBelongingToGroup(int groupId)
        {
            return _userMapper.Map(_enrollmentService.GetUsersNotBelongingToGroup(groupId));
        }

        [HttpPost]
        [Route("enrollUsersToGroup")]
        public async Task<IHttpActionResult> EnrollUsersToGroup(UsersForEnrollmentModel data)
        {
            if(data == null)
            {
                throw new ArgumentNullException("Parameter could not be null", "data");
            }
            var users = _userMapper.Map(data.UserModels);
            _enrollmentService.EnrollUsersToGroup(users, data.GroupId);
            await _wordSuiteService.CopyWordsuitesForUsersByGroupAsync(users, data.GroupId);
            _wordProgressService.CopyProgressesForUsersInGroup(users, data.GroupId);
            return Ok();
        }

        public IHttpActionResult Delete(int enrollmentId)
        {
            _wordProgressService.RemoveProgressesForEnrollment(enrollmentId);
            _wordSuiteService.RemoveWordSuitesForEnrollment(enrollmentId);
            _enrollmentService.DeleteById(enrollmentId);
            return Ok();
        }
    }
}