using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Factory;

namespace WorldOfWords.Domain.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public EnrollmentService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public List<Enrollment> GetByGroupId(int groupId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return uow.EnrollmentRepository.GetAll().Where(e => e.GroupId == groupId).Include(e => e.User).OrderBy(e => e.User.Name).ToList();
            }
        }

        public List<User> GetUsersNotBelongingToGroup(int groupId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var group = uow.GroupRepository.GetById(groupId);
                if (group == null)
                {
                    return null;
                }
                var course = group.Course;
                var usersCurrentlyInGroup = uow.EnrollmentRepository.GetAll().Where(e => e.GroupId == groupId).Select(e => e.User);
                //User to add to this group must:
                return uow.UserRepository.GetAll().Where(u =>
                    //have student role
                    u.Roles.Select(r => r.Name).Contains("Student")
                        //not be in this group already
                    && !usersCurrentlyInGroup.Any(u2 => u2.Id == u.Id)
                        //not be subscribed on course, that this group is assigned to, already
                    && u.Enrollments.Select(e => e.Group.Course).FirstOrDefault(c => c.Id == course.Id) == null
                        //and you cannot subscribe yourself to your group
                    && u.Id != group.OwnerId).ToList();
            }
        }

        public void EnrollUsersToGroup(List<User> users, int groupId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var enrollmentsToAdd = users.Select(user => new Enrollment
                {
                    GroupId = groupId,
                    UserId = user.Id,
                    Date = DateTime.Now
                }).ToList();
                uow.EnrollmentRepository.Add(enrollmentsToAdd);
                uow.Save();
            }
        }

        public void DeleteById(int enrollmentId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                uow.EnrollmentRepository.Delete(enrollmentId);
                uow.Save();
            }
        }
    }
}