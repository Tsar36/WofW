using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class GroupMapper: IGroupMapper
    {
        public Group Map(GroupModel groupModel)
        {
            Group newGroup = new Group()
            {
                Name = groupModel.Name,
                CourseId = groupModel.CourseId,
                OwnerId = groupModel.OwnerId
            };
            return newGroup;
        }

        public GroupModel Map(Group group)
        {
            return new GroupModel()
            {
                Name = group.Name,
                CourseId = group.CourseId,
                OwnerId = group.OwnerId
            };
        }

        public List<GroupModel> Map(List<Group> groups)
        {
            List<GroupModel> groupModels = new List<GroupModel>();
            foreach(Group group in groups)
            {
                groupModels.Add(Map(group));
            }
            return groupModels;
        }
    }
}
