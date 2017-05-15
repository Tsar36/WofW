using System.Collections.Generic;
using System.Threading.Tasks;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Domain.Services
{
    public interface IGroupService
    {
        List<Group> GetAll(int userId);
        Task<List<Group>> GetAllToSubscribeAsync(int userId); 
        bool ContainsGroupName(GroupModel groupModel);
        void Add(Group groupModel);
        Group GetById(int groupId, int userId);
        void DeleteById(int groupId);
    }
}