using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class GroupForListingMapper: IGroupForListingMapper
    {
        public GroupForListingModel Map(Group group)
        {
            return new GroupForListingModel()
            {
                Id = group.Id,
                Name = group.Name,
                CourseId = group.Course.Id,
                CourseName = group.Course.Name,
                OwnerId = group.OwnerId,
                OwnerName = group.Owner.Name
            };
        }

        public List<GroupForListingModel> Map(List<Group> groups)
        {
            List<GroupForListingModel> models = new List<GroupForListingModel>();
            foreach(var group in groups)
            {
                models.Add(Map(group));
            }
            return models;
        }
    }
}
