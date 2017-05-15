using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IGroupForListingMapper
    {
        GroupForListingModel Map(Group group);
        List<GroupForListingModel> Map(List<Group> groups);
    }
}
