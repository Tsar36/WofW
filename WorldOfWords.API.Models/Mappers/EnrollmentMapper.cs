using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models.Mappers
{
    public class EnrollmentMapper: IEnrollmentMapper
    {
        readonly IUserForListingMapper _userMapper;
        public EnrollmentMapper(IUserForListingMapper userMapper)
        {
            _userMapper = userMapper;
        }
        public EnrollmentModel Map(Enrollment enrollment)
        {
            return new EnrollmentModel()
            {
                Id = enrollment.Id,
                Date = string.Format("{0:dd.MM.yyyy}", enrollment.Date),
                GroupId = enrollment.GroupId,
                User = _userMapper.Map(enrollment.User)
            };
        }

        public List<EnrollmentModel> Map(List<Enrollment> enrollments)
        {
            List<EnrollmentModel> enrollmentModels = new List<EnrollmentModel>();
            foreach(var enrollment in enrollments)
            {
                enrollmentModels.Add(Map(enrollment));
            }
            return enrollmentModels;
        }
    }
}
