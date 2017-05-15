using System.Collections.Generic;
using WorldOfWords.Domain.Models;
using System.Threading.Tasks;
using WorldOfWords.API.Models;
using WorldOfWords.API.Models.Models;


namespace WorldOfWords.Domain.Services
{
    public interface ITagService
    {
        int Exists(string name);        
        Task<List<TagModel>> GetTopBySearchTagAsync(string searchTag, int count);
        Task<Tag> GetFirstTagByNameAsync(string tagName);
        Task<Tag> GetTagByIdAsync(int id);
        Task<int> AddAsync(TagModel tag);
        Task AddToWordAsync(int wordId, int tagId);
        Task AddRangeToWordAsync(int wordId, List<int> tags);
        Task<bool> RemoveTagFromWordAsync(int wordId, int tagId);
        Task<bool> RemoveIdRangeAsync(int wordId, List<int> tagIds);
    }
}
