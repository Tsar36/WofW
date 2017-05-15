using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IGroupMapper
    {
        Group Map(GroupModel groupMode);
        GroupModel Map(Group group);
        List<GroupModel> Map(List<Group> groups);
    }
}
