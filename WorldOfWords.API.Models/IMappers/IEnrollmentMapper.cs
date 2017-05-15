using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IEnrollmentMapper
    {
        EnrollmentModel Map(Enrollment enrollment);
        List<EnrollmentModel> Map(List<Enrollment> enrollments);
    }
}
