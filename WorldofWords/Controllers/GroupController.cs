using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services;

namespace WorldofWords.Controllers
{
    [WowAuthorization(Roles = "Teacher")]
    [RoutePrefix("api/Group")]
    public class GroupController : BaseController
    {
        private readonly IGroupForListingMapper _groupForListingMapper;
        private readonly IGroupService _groupService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IWordProgressService _wordProgressService;
        private readonly IWordSuiteService _wordsuiteService;
        private readonly ICourseService _courseService;
        private readonly ICourseForGroupMapper _courseMapper;
        private readonly IGroupMapper _groupMapper;

        public GroupController(IGroupService groupService, IGroupForListingMapper groupForListingMapper, IEnrollmentService enrollmentService, 
            IWordProgressService wordProgressService, IWordSuiteService wordsuiteService, ICourseService courseService,
            ICourseForGroupMapper courseMapper, IGroupMapper groupMapper)
        {
            _groupService = groupService;
            _groupForListingMapper = groupForListingMapper;
            _enrollmentService = enrollmentService;
            _wordProgressService = wordProgressService;
            _wordsuiteService = wordsuiteService;
            _courseService = courseService;
            _courseMapper = courseMapper;
            _groupMapper = groupMapper;
        }

        public List<GroupForListingModel> Get()
        {
            return _groupForListingMapper.Map(_groupService.GetAll(UserId));
        }

        public GroupForListingModel Get(int groupId)
        {
            var group = _groupService.GetById(groupId, UserId);
            if (group == null)
            {
                return null;
            }

            return _groupForListingMapper.Map(group);
        }

        [Route("getCourses")]
        public List<CourseForGroupModel> GetCourses()
        {
            return _courseMapper.Map(_courseService.GetAllCourses(UserId));
        }

        [Route("AllToSubscribe/{userId}")]
        public async Task<List<GroupForListingModel>> GetAllToSubscribe(int userId)
        {
            return _groupForListingMapper.Map(await _groupService.GetAllToSubscribeAsync(userId));
        }

        public IHttpActionResult Post(GroupModel newGroup)
        {
            if (newGroup == null)
            {
                throw new ArgumentNullException("Parameter could not be null", "newGroup");
            }

            if (!_groupService.ContainsGroupName(newGroup))
            {
                var groupToAdd = _groupMapper.Map(newGroup);
                _groupService.Add(groupToAdd);
                return Ok();
            }
            return BadRequest(string.Format("Group {0} already exist!", newGroup.Name));
        }

        public IHttpActionResult Delete(int groupId)
        {
            List<Enrollment> enrollments = _enrollmentService.GetByGroupId(groupId);
            foreach (var enrollment in enrollments)
            {
                _wordProgressService.RemoveProgressesForEnrollment(enrollment.Id);
                _wordsuiteService.RemoveWordSuitesForEnrollment(enrollment.Id);
            }
            _groupService.DeleteById(groupId);
            return Ok();
        }
    }
}